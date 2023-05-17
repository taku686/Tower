using System;
using Data;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdMobManager : MonoBehaviour
{
    private BannerView _bannerViewTop;
    private GameObject icon;
    private bool nativeAdLoaded;
    private NativeAd nativeAd;
    private readonly Vector3 _adPos1 = new(0, 3.11f, 0);
    private readonly Vector3 _adPos2 = new(0, -4.36f, 0);
    private readonly Vector3 _adScale1 = new(4.59f, 3.36f, 1);
    private readonly Vector3 _adScale2 = new(4.68f, 1, 1);


    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        RequestNative();
        //RequestBanner();
    }

    private void Update()
    {
        if (this.nativeAdLoaded)
        {
            this.nativeAdLoaded = false;
            // Get Texture2D for icon asset of native ad.
            Texture2D iconTexture = this.nativeAd.GetIconTexture();

            icon = GameObject.CreatePrimitive(PrimitiveType.Quad);
            icon.transform.position = _adPos1;
            icon.transform.localScale = _adScale1;
            icon.GetComponent<Renderer>().material.mainTexture = iconTexture;

            // Register GameObject that will display icon asset of native ad.
            if (!this.nativeAd.RegisterIconImageGameObject(icon))
            {
                // Handle failure to register ad asset.
            }
        }
    }

    private void RequestBanner()
    {
        if (_bannerViewTop != null)
        {
            _bannerViewTop.Destroy();
            _bannerViewTop = null;
        }

        var bannerHeight = 400 * Screen.dpi / 160.0f;
        AdSize adSize = new AdSize(360, (int)bannerHeight);
        _bannerViewTop = new BannerView(GameCommonData.BannerAdUnitId, AdSize.SmartBanner, AdPosition.Center);
        AdRequest request = new AdRequest.Builder().Build();
        _bannerViewTop.LoadAd(request);
    }

    private void RequestNative()
    {
        AdLoader adLoader = new AdLoader.Builder(GameCommonData.NativeAdUnitId)
            .ForNativeAd()
            .Build();
        adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded;
        adLoader.OnAdFailedToLoad += this.HandleNativeAdFailedToLoad;
        adLoader.LoadAd(new AdRequest.Builder().Build());
    }

    private void HandleNativeAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("Native ad failed to load: " + args.LoadAdError.GetMessage());
    }

    private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
    {
        Debug.Log("Native ad loaded.");
        this.nativeAd = args.nativeAd;
        this.nativeAdLoaded = true;
    }
}