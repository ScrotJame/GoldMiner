using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.Events;
using System.Collections;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class GoogleAdsManager : MonoBehaviour
{

    [SerializeField]
    private string _bannerId;
    [SerializeField]
    private string _interstitialId;
    [SerializeField]
    private string _rewardedAdId; // Biến riêng cho ID quảng cáo thưởng
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;
    BannerView _bannerView;

    //rewarded ad
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
            LoadRewardedAd(); // Tải quảng cáo thưởng ngay khi khởi tạo
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
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_bannerId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;
                RegisterEventHandlers(_interstitialAd);
                RegisterReloadHandler(_interstitialAd);
            });
    }
    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }
    public bool IsBannerAdLoaded()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            return true;
        }
        else return false;
    }
    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
            if (interstitialAdEvent != null)
            {
                interstitialAdEvent.Invoke();
            }
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            if (AudioManager.Instance != null && AudioManager.Instance.background != null)
            {
                AudioManager.Instance.background.Stop();
            }
            else
            {
                Debug.LogWarning("AudioManager or background is null!");
            }
        };

        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");

            AudioManager.Instance.background.Play();
            LoadInterstitialAd();
            onAdClosedGoToShop?.Invoke();
            onAdClosedGoToShop = null;
        };

        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
        };
    }
    private void RegisterReloadHandler(InterstitialAd interstitialAd)
    {
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial Ad full screen content closed.");
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
        RewardedAd.Load(_rewardedAdId, adRequest, // Sử dụng _rewardedAdId
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError($"Rewarded ad failed to load. Error: {error?.GetMessage() ?? "Unknown error"}");
                    return;
                }

                Debug.Log($"Rewarded ad loaded successfully. Response: {ad.GetResponseInfo()}");
                _rewardedAd = ad;
                RewardedEventHandlers(_rewardedAd);
                RewardedReloadHandler(_rewardedAd);
            });
    }

    public void ShowRewardedAd()
    {
        const string rewardMsg = "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
                interwRewardEvent?.Invoke();
                UIManager.instance.lastAdLevel = UIManager.instance.currentLevelIndex;
                UIManager.instance.UpdateAdsRewardButton();
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
               Pod.instance.AddDynamite();
                break;
            case "Streng":
                AddStreng();
                break;
        }
    }
    public void AddStreng()
    {
        ItemManager.Instance.AddItem("Streng", null, () => { });
    }
    private void RewardedEventHandlers(RewardedAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Rewarded ad paid {adValue.Value} {adValue.CurrencyCode}.");
        };
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
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
            Debug.Log("Rewarded Ad full screen content closed.");
            LoadRewardedAd(); 
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError($"Rewarded ad failed to open full screen content with error: {error}");
            LoadRewardedAd(); 
        };
    }
}

   