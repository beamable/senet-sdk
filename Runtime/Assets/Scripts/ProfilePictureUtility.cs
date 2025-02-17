using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public static class ProfilePictureUtility
{
    public static async Task<string> FetchProfilePictureUrl(long playerId)
    {
        try
        {
            var beamContext = await BeamContext.Default.Instance;
            var stats = await beamContext.Api.StatsService.GetStats("client", "public", "player", playerId);
            return stats.GetValueOrDefault("profile_url", "");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching profile picture URL: {ex.Message}");
            return "";
        }
    }

    public static async Task LoadImageFromUrl(string url, Image targetImage)
    {
        if (string.IsNullOrEmpty(url) || targetImage == null) return;

        try
        {
            using var webRequest = UnityWebRequestTexture.GetTexture(url);
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                var texture = DownloadHandlerTexture.GetContent(webRequest);
                if (texture != null)
                {
                    var sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );

                    targetImage.sprite = sprite;
                    AdjustImageToFill(sprite, targetImage);
                }
            }
            else
            {
                Debug.LogError($"Failed to load image from URL: {url}, Error: {webRequest.error}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error loading image from URL: {ex.Message}");
        }
    }

    public static void AdjustImageToFill(Sprite sprite, Image targetImage)
    {
        if (sprite == null || targetImage == null) return;

        var rectTransform = targetImage.rectTransform;
        var imageRatio = (float)sprite.texture.width / sprite.texture.height;
        var parentSize = rectTransform.parent.GetComponent<RectTransform>().rect;

        var parentRatio = parentSize.width / parentSize.height;

        rectTransform.sizeDelta = imageRatio > parentRatio
            ? new Vector2(parentSize.height * imageRatio, parentSize.height)
            : new Vector2(parentSize.width, parentSize.width / imageRatio);
    }
    
    
}