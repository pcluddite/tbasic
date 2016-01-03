using System;
using System.IO;
using System.Security;

namespace Tbasic {
    /// <summary>
    /// A class for handling Tbasic errors
    /// </summary>
    public class TException : Exception {

        /// <summary>
        /// The status code of the exception
        /// </summary>
        public int StatusCode {
            get {
                return int.Parse(Message.Remove(Message.IndexOf(' ')));
            }
        }

        /// <summary>
        /// The description of the exception (without the status code)
        /// </summary>
        public string Description {
            get {
                return Message.Substring(Message.IndexOf(' ')).Trim();
            }
        }

        /// <summary>
        /// Initializes a new TException object
        /// </summary>
        /// <param name="code">Tbasic status code</param>
        /// <param name="msg">message</param>
        public TException(int code, string msg)
            : base(GetMessage(code, msg, true)) {
        }

        /// <summary>
        /// Initializes a new TException object
        /// </summary>
        /// <param name="code">Tbasic status code</param>
        /// <param name="msg">message</param>
        /// <param name="prependGeneric">indicates whether a generic message should be prepended to this one</param>
        public TException(int code, string msg, bool prependGeneric)
            : base(GetMessage(code, msg, prependGeneric)) {
        }

        /// <summary>
        /// Initializes a new TException object
        /// </summary>
        /// <param name="ex">the exception that this exception is processing</param>
        public TException(Exception ex)
            : base(GetMessage(ex)) {
        }

        /// <summary>
        /// Initializes a new TException object
        /// </summary>
        /// <param name="code">Tbasic status code</param>
        public TException(int code)
            : base(GetMessage(code)) {
        }

        internal static string GetMessage(int code) {
            switch (code) {
                case 200:
                    return "OK";
                case 201:
                    return "201 Created";
                case 202:
                    return "202 Accepted";
                case 204:
                    return "204 No Content";
                case 206:
                    return "206 Completed with warnings";
                case 400:
                    return "400 Bad Request";
                case 401:
                    return "401 Unauthorized";
                case 403:
                    return "403 No user access";
                case 404:
                    return "404 Not Found";
                case 409:
                    return "409 Conflict";
                case 423:
                    return "423 Locked";
                case 501:
                    return "501 Not Implemented";
                case 502:
                    return "502 Bad Gateway";
                case 507:
                    return "507 Insufficient Memory";
                default:
                    return "500 Generic Error";
            }
        }

        internal static string GetMessage(Exception ex) {
            if (ex is TException) {
                return ex.Message;
            }
            else if (ex is ArgumentException || ex is ArgumentNullException) {
                return GetMessage(400);
            }
            else if (ex is PathTooLongException || ex is NotSupportedException) {
                return GetMessage(400, ex.Message);
            }
            else if (ex is UnauthorizedAccessException || ex is SecurityException || ex.Message.Contains("Logon failure")) {
                return GetMessage(403);
            }
            else if (ex.GetType().Name.Contains("NotFound")) {
                return GetMessage(404);
            }
            else if (ex is NotImplementedException) {
                return GetMessage(501, ex.Message, false);
            }
            else if (ex is IOException) {
                return GetMessage(423, ex.Message);
            }
            return GetMessage(500, ex.Message);
        }

        private static string GetMessage(int code, string msg, bool prependGeneric) {
            if (prependGeneric) {
                return GetMessage(code) + ": " + msg;
            }
            else {
                return code + " " + msg;
            }
        }

        internal static string GetMessage(int code, string msg) {
            return GetMessage(code, msg, true);
        }
    }
}
