using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;

// ReSharper disable InvertIf

namespace Serilog.Sinks.YouTrack.Services
{
    /// <remarks>YouTrack import API for bulk issue creation requires low-level update access, thus not bothered implementing support.</remarks>    
    public sealed class YouTrackReporter : IDisposable, IYouTrackReporter
    {
        private readonly HttpClient client;
        private readonly Func<Task<bool>> auth;
        private readonly Action onDispose;

        /// <summary>
        /// YouTrack issue reporter
        /// </summary>
        /// <param name="username">YouTrack username</param>
        /// <param name="password">YouTrack password</param>
        /// <param name="youTrackUri">YouTrack endpoint</param>
        /// <param name="authImmediately">If set to true, ensure auth cookie can be obtained on construction (synchronous) or throw.</param>
        public YouTrackReporter(string username, string password, Uri youTrackUri, bool authImmediately = false)
            : this(username, SecureStringHelper.ToSecureString(password), youTrackUri, authImmediately)
        {
        }

        /// <summary>
        /// YouTrack issue reporter
        /// </summary>
        /// <param name="username">YouTrack username</param>
        /// <param name="password">YouTrack password</param>
        /// <param name="youTrackUri">YouTrack endpoint</param>
        /// <param name="authImmediately">If set to true, ensure auth cookie can be obtained on construction (synchronous) or throw.</param>
        public YouTrackReporter(string username, SecureString password, Uri youTrackUri, bool authImmediately = false)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException(nameof(username));
            }

            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (youTrackUri == null)
            {
                throw new ArgumentNullException(nameof(youTrackUri));
            }

            var handler = new HttpClientHandler()
            {
                CookieContainer = new CookieContainer()
            };

            auth = async () =>
            {
                var cookie = handler.CookieContainer.GetCookies(youTrackUri)
                    .OfType<Cookie>()
                    .FirstOrDefault(x => x.Name.Equals(YouTrackContracts.AuthCookie, StringComparison.Ordinal));

                if (cookie == null || cookie.Expired)
                {
                    var response = await client
                        .PostAsync(YouTrackContracts.Auth, new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("login", username),
                            new KeyValuePair<string, string>("password", SecureStringHelper.ToString(password)),
                        }))
                        .ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new InvalidOperationException(
                            $"Authenticating through {response.RequestMessage.RequestUri}. Response {(int) response.StatusCode}: {response.ReasonPhrase}{Environment.NewLine}Body: {await response.Content.ReadAsStringAsync()}");
                    }

                    return true;
                }

                return true;
            };

            client = new HttpClient(handler, true)
            {
                BaseAddress = youTrackUri
            };

            // ReSharper disable once ImplicitlyCapturedClosure
            void OnDispose()
            {
                password.Dispose();
                client.Dispose();
            }

            if (authImmediately)
            {
                try
                {
                    auth().Wait();
                }
                catch
                {
                    OnDispose();
                    throw;
                }
            }

            onDispose = OnDispose;
        }

        /// <summary>
        /// Create new issue in YouTrack.
        /// </summary>
        /// <param name="project">YouTrack project ID.</param>
        /// <param name="summary">Issue summary.</param>
        /// <param name="description">Issue description.</param>
        /// <param name="issueType">YouTrack issue type. If omitted, then YouTrack project default is used.</param>
        /// <returns></returns>
        public async Task<Uri> CreateIssue(string project, string summary, string description, string issueType = null)
        {
            if (string.IsNullOrEmpty(project))
            {
                throw new ArgumentException("Specify project", nameof(project));
            }

            if (string.IsNullOrEmpty(summary))
            {
                throw new ArgumentException("Specify summary", nameof(project));
            }

            await auth().ConfigureAwait(false);

            return await CreateIssueImpl(project, summary, description, issueType).ConfigureAwait(false);
        }

        private async Task<Uri> CreateIssueImpl(string project, string summary, string description, string issueType)
        {
            Uri uri;
            using (var response = await client.PutAsync(YouTrackContracts.Issue, new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("project", project),
                new KeyValuePair<string, string>("summary", summary),
                new KeyValuePair<string, string>("description", description),
            })))
            {
                if (response.StatusCode != HttpStatusCode.Created)
                {
                    throw new InvalidOperationException(
                        $"Creating issue through {response.RequestMessage.RequestUri}. Response {(int) response.StatusCode}: {response.ReasonPhrase}{Environment.NewLine}Body: {await response.Content.ReadAsStringAsync()}");
                }

                uri = response.Headers.Location;
            }

            if (issueType != null)
            {
                await ExecuteAgainstIssue(uri, $"type {issueType}").ConfigureAwait(false);
            }

            return uri;
        }

        /// <summary>
        /// Execute YouTrack command against issue.
        /// <see href="https://www.jetbrains.com/help/youtrack/incloud/Command-Reference.html">YouTrack Command Reference</see>.
        /// </summary>
        /// <param name="issue">Issue uri from CreateIssue.</param>
        /// <param name="command">Command to execute.</param>
        /// <param name="comment">Comment to add.</param>
        /// <returns>Uri of the provided issue.</returns>
        public async Task<Uri> ExecuteAgainstIssue(Uri issue, string command, string comment = null)
        {
            var cmd = new KeyValuePair<string, string>("command", command);
            var cmds = !string.IsNullOrEmpty(comment)
                ? new[] {cmd, new KeyValuePair<string, string>("comment", comment)}
                : new[] {cmd};

            using (var response = await client.PostAsync($"{issue.PathAndQuery}/execute", new FormUrlEncodedContent(cmds)).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($@"Executing command ""{command}"" through {response.RequestMessage.RequestUri}. Response {(int)response.StatusCode}: {response.ReasonPhrase}{Environment.NewLine}Body: {await response.Content.ReadAsStringAsync()}");
                }
                return issue;
            }
        }

        /// <summary>
        /// Dispose any managed resources (HttpClient)
        /// </summary>
        public void Dispose()
        {
            onDispose();
        }
    }
}