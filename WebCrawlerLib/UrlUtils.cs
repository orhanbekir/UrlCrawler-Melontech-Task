using System;

namespace WebCrawlerLib
{
    public static class UrlUtils
    {
        /// <summary>
        /// Combine base url with additional url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string CombineUrl(string baseUrl, string url)
        {
            try
            {
                Uri baseUri = new Uri(baseUrl);
                Uri myUri = new Uri(baseUri, url);
                return myUri.ToString();
            }
            catch (Exception e)
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Is Start With http:// or https://
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsAbsoluteUrl(string url)
        {
            if (url.StartsWith("http://") || url.StartsWith("https://"))
                return true;
            
            return false;
        }

        /// <summary>
        /// Build Url Check Is Absolute or Combine Url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string BuildUrl(string baseUrl, string url)
        {
            if (IsAbsoluteUrl(url))
                return url;

            return CombineUrl(baseUrl, url);
        }


        public static bool IsSameHost(string url1, string url2)
        {
            Uri baseUri = new Uri(url1);
            Uri uri = new Uri(url2);

            return baseUri.Host.Equals(uri.Host);
        }
    }
}
