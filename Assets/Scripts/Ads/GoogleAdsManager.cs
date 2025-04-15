using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.Events;
using System.Collections;

public class GoogleAdsManager : MonoBehaviour
{
    [SerializeField]
    private string _bannerId;
    [SerializeField]
    private string _interstitialId;
    [SerializeField]
    private string _rewardedAdId;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;
    BannerView _bannerView;

    private string _adsrewardType;
    private string blindBoxReward;
    [Header("Test Ads")]
    public UnityAction interwRewardEvent;
    public UnityAction interstitialAdEvent;
    public UnityAction onAdClosedGoToShop;

    void Start()
    {
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            LoadInterstitialAd();
            LoadRewardedAd();
            interwRewardEvent += () => GiveRewarded();
        });

        string[] Adsrewards = { "Dynamite", "Streng" };
        _adsrewardType = Adsrewards[UnityEngine.Random.Range(0, Adsrewards.Length)];
    }

    public string GetAdsRewardType()
    {
        return _adsrewardType;
    }

    public void LoadInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        var adRequest = new AdRequest();
        InterstitialAd.Load(_bannerId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad with error : " + error);
                    return;
                }

                _interstitialAd = ad;
                RegisterEventHandlers(_interstitialAd);
                RegisterReloadHandler(_interstitialAd);
            });
    }

    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }

    public bool IsBannerAdLoaded()
    {
        return _interstitialAd != null && _interstitialAd.CanShowAd();
    }

    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Interstitial ad paid {adValue.Value} {adValue.CurrencyCode}.");
            interstitialAdEvent?.Invoke();
        };
        interstitialAd.OnAdImpressionRecorded += () =>
        {
        };
        interstitialAd.OnAdClicked += () => { 
        };
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            if (AudioManager.Instance != null && AudioManager.Instance.background != null)
            {
                AudioManager.Instance.background.Stop();
            }
            else
            {
                Debug.LogWarning("AudioManager hoặc background là null!");
            }
        };
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            AudioManager.Instance?.background.Play();
            LoadInterstitialAd();
            onAdClosedGoToShop?.Invoke();
            onAdClosedGoToShop = null;
        };
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content with error : " + error);
        };
    }

    private void RegisterReloadHandler(InterstitialAd interstitialAd)
    {
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            LoadInterstitialAd();
            onAdClosedGoToShop?.Invoke();
        };
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content with error: " + error);
            LoadInterstitialAd();
        };
    }

    public void LoadRewardedAd()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        var adRequest = new AdRequest();
        RewardedAd.Load(_rewardedAdId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError($"Rewarded ad failed to load. Error: {error?.GetMessage() ?? "Unknown error"}");
                    return;
                }

                _rewardedAd = ad;
                RewardedEventHandlers(_rewardedAd);
                RewardedReloadHandler(_rewardedAd);
            });
    }

    public void ShowRewardedAd()
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            Debug.Log("Showing rewarded ad.");
            _rewardedAd.Show((Reward reward) =>
            {
                interwRewardEvent?.Invoke();
                if (UIManager.instance != null)
                {
                    UIManager.instance.UpdateAdsRewardButton();
                }
            });
        }
        else
        {
            Debug.LogError("Rewarded ad is not ready yet.");
            LoadRewardedAd();
            StartCoroutine(TryShowAdAfterDelay(2f));
        }
    }

    private IEnumerator TryShowAdAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowRewardedAd();
    }

    public void GiveRewarded()
    {
        blindBoxReward = GetAdsRewardType();
        ApplyBlindBoxReward(blindBoxReward);
    }

    private void ApplyBlindBoxReward(string reward)
    {
        if (string.IsNullOrEmpty(reward)) return;

        switch (reward)
        {
            case "Dynamite":
                Pod.instance?.AddDynamite();
                if (UIManager.instance != null)
                {
                    UIManager.instance.UpdateDynamiteCount(ItemManager.Instance.GetItemCount("Dynamite"));
                }
                break;
            case "Streng":
                AddStreng();
                break;
        }
    }

    public void AddStreng()
    {
        ItemManager.Instance?.AddItem("Streng", null, () => { });
    }

    private void RewardedEventHandlers(RewardedAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Rewarded ad paid {adValue.Value} {adValue.CurrencyCode}.");
        };
        ad.OnAdImpressionRecorded += () =>
        {
        };
        ad.OnAdClicked += () =>
        {
        };
        ad.OnAdFullScreenContentOpened += () =>
        {
        };
        ad.OnAdFullScreenContentClosed += () =>
        {
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError($"Rewarded ad failed to open full screen content with error: {error}");
        };
    }

    private void RewardedReloadHandler(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            LoadRewardedAd();
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError($"Rewarded ad failed to open full screen content with error: {error}");
            LoadRewardedAd();
        };
    }
}