using Data;
using GoogleMobileAds.Api;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdMobManager : MonoBehaviour
{
    private BannerView _bannerViewTop;
    private GameObject icon;
    private bool nativeAdLoaded;
    private NativeAd nativeAd;

    public RawImage AdIconTexture;
    public TextMeshProUGUI AdHeadline;
    public TextMeshProUGUI AdDescription;
    public TextMeshProUGUI adCallToAction;
    public GameObject AdLoaded;
    public GameObject AdLoading;
    private const string AssetName = "Ad1";
    public TextMeshProUGUI debugText;

    void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            RequestNativeAd();
            RequestBanner();
        });
    }

    private void RequestNativeAd()
    {
        AdLoader adLoader = new AdLoader.Builder(GameCommonData.NativeAdUnitId)
            .ForNativeAd()
            .Build();
        adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded;
        adLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;
        adLoader.LoadAd(new AdRequest.Builder().Build());
    }

    private void RequestBanner()
    {
        if (_bannerViewTop != null)
        {
            _bannerViewTop.Destroy();
            _bannerViewTop = null;
        }

        _bannerViewTop = new BannerView(GameCommonData.BannerAdUnitId, AdSize.SmartBanner, AdPosition.Bottom);
        AdRequest request = new AdRequest.Builder().Build();
        _bannerViewTop.LoadAd(request);
        HideBanner();
    }

    public void ShowBanner()
    {
        _bannerViewTop.Show();
    }

    public void HideBanner()
    {
        _bannerViewTop.Hide();
    }


    private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        debugText.text = "Native ad failed to load: " + args.LoadAdError.GetMessage();
        Debug.LogError("Native ad failed to load: " + args.LoadAdError.GetMessage());
    }


    private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
    {
        debugText.text = "Native ad loaded.";
        Debug.Log("Native ad loaded.");
        this.nativeAd = args.nativeAd;
        AdIconTexture.texture = nativeAd.GetIconTexture();
        AdHeadline.text = nativeAd.GetHeadlineText();
        AdDescription.text = nativeAd.GetBodyText();
        adCallToAction.text = nativeAd.GetCallToActionText();
        if (!nativeAd.RegisterIconImageGameObject(AdIconTexture.gameObject))
        {
            debugText.text = "error registering icon";
            Debug.Log("error registering icon");
        }

        if (!nativeAd.RegisterHeadlineTextGameObject(AdHeadline.gameObject))
        {
            debugText.text = "error registering headline";
            Debug.Log("error registering headline");
        }

        if (!nativeAd.RegisterBodyTextGameObject(AdDescription.gameObject))
        {
            debugText.text = "error registering description";
            Debug.Log("error registering description");
        }

        nativeAd.RegisterIconImageGameObject(AdIconTexture.gameObject);
        nativeAd.RegisterHeadlineTextGameObject(AdHeadline.gameObject);
        nativeAd.RegisterCallToActionGameObject(adCallToAction.gameObject);
        nativeAd.RegisterAdvertiserTextGameObject(AdDescription.gameObject);
        //disable loading and enable ad object
        AdLoaded.gameObject.SetActive(true);
        AdLoading.gameObject.SetActive(false);
    }

    private void HandleCustomNativeAdClicked(CustomNativeAd customNativeAd, string assetName)
    {
        Debug.Log("Custom Native ad asset with name " + assetName + " was clicked.");
        customNativeAd.PerformClick(AssetName);
    }
}