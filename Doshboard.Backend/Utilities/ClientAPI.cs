﻿using System.Net;

namespace Doshboard.Backend.Utilities
{
    public static class ClientAPI
    {
        public static async Task<T?> GetAsync<T>(this HttpClient client, string url)
        {
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return default;
            if (response.StatusCode == HttpStatusCode.NoContent)
                return default;
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public static async Task<T?> PostAsync<T>(this HttpClient client, string url)
        {
            var response = await client.PostAsync(url, null);

            if (!response.IsSuccessStatusCode)
                return default;
            if (response.StatusCode == HttpStatusCode.NoContent)
                return default;
            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
