using Data;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdMobManager : MonoBehaviour
{
    private BannerView _bannerViewTop;

    void Start()
    {
        RequestBanner();
    }

    private void RequestBanner()
    {
        if (_bannerViewTop != null)
        {
            _bannerViewTop.Destroy();
            _bannerViewTop = null;
        }

        AdSize adSize = new AdSize(320, 480);
        _bannerViewTop = new BannerView(GameCommonData.AdUnitId, AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(400), AdPosition.Center);
        AdRequest request = new AdRequest.Builder().AddKeyword("unity-admob-sample").Build();
        _bannerViewTop.LoadAd(request);
    }
}