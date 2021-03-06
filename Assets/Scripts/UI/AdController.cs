﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine.UI;

public class AdController : MonoBehaviour
{
	RewardBasedVideoAd rewardBasedVideo;
	bool rewardBasedEventHandlersSet = false;
	bool adIsLoadingOrWatching = false;
	bool isStarted = false;
	Text kittyzText, adText;
	Button adButton;

	void Start ()
	{
		//init kittyz
		kittyzText = GameObject.Find ("KittyzText").GetComponentInChildren<Text> ();
		kittyzText.text = ApplicationController.ac.playerData.kittyz.ToString ();
		//init ad
		adText = GetComponentInChildren<Text> ();
		rewardBasedVideo = RewardBasedVideoAd.Instance;
		//init ad button
		adButton = GetComponent<Button> ();
		if (rewardBasedVideo.IsLoaded ())
			//enableButton ();
			RequestRewardBasedVideo ();
		else
			enableButton (false);
		isStarted = true;
	}

	void Update ()
	{
		if (!adIsLoadingOrWatching && !rewardBasedVideo.IsLoaded ()) {
			adIsLoadingOrWatching = true;
			RequestRewardBasedVideo ();
		} else if (rewardBasedVideo.IsLoaded () && !adButton.IsInteractable ())
			enableButton ();
	}

	public void RequestRewardBasedVideo ()
	{
		string adUnitId = "ca-app-pub-3940256099942544/5224354917"; //testing ID
		//string adUnitId = Config.adUnitId;

		rewardBasedVideo = RewardBasedVideoAd.Instance;

		AdRequest request = new AdRequest.Builder ()
			.AddTestDevice (AdRequest.TestDeviceSimulator)       // Simulator.
			.AddTestDevice (Config.myTestDevice1) 
			.AddTestDevice (Config.myTestDevice1Caps) 
			.Build ();
		rewardBasedVideo.LoadAd (request, adUnitId);

		// update ad button
		enableButton (false, "AD_LOADING");
		// Reward based video instance is a singleton. Register handlers once to avoid duplicate events.
		if (!rewardBasedEventHandlersSet) {
			// Ad event fired when the rewarded video ad has been received.
			rewardBasedVideo.OnAdLoaded += HandleOnAdLoaded;
			rewardBasedVideo.OnAdFailedToLoad += HandleOnAdFailedToLoad;
			rewardBasedVideo.OnAdOpening += HandleOnAdOpening;
			rewardBasedVideo.OnAdStarted += HandleOnAdStarted;
			rewardBasedVideo.OnAdRewarded += HandleOnAdRewarded;
			rewardBasedVideo.OnAdClosed += HandleOnAdClosed;
			rewardBasedVideo.OnAdLeavingApplication += HandleOnAdLeavingApplication;

			//rewardBasedVideo.rewardBasedEventHandlersSet = true;
			rewardBasedEventHandlersSet = true;
		}
	}


	public void ShowAd ()
	{
		if (rewardBasedVideo.IsLoaded ()) {
			adIsLoadingOrWatching = true;
			rewardBasedVideo.Show ();
			//enableButton (false, "Ad opening...");
		}
	}

	public void HandleOnAdLoaded (object sender, EventArgs args)
	{
		adIsLoadingOrWatching = false;
		enableButton ();
	}

	public void HandleOnAdFailedToLoad (object sender, AdFailedToLoadEventArgs args)
	{
		adIsLoadingOrWatching = false;
	}

	public void HandleOnAdOpening (object sender, EventArgs args)
	{
		enableButton (false, "AD_OPENING");
	}

	public void HandleOnAdStarted (object sender, EventArgs args)
	{
		adIsLoadingOrWatching = true;
		enableButton (false, "AD_PLAYING");
	}

	public void HandleOnAdClosed (object sender, EventArgs args)
	{
		adIsLoadingOrWatching = false;
		rewardBasedVideo = RewardBasedVideoAd.Instance;
	}

	public void HandleOnAdRewarded (object sender, Reward args)
	{
		string type = args.Type;
		double amount = args.Amount;
		Debug.Log ("User rewarded with: " + amount.ToString () + " " + type);
		ApplicationController.ac.playerData.updateKittys ((int)amount * 2, kittyzText, true);
		adIsLoadingOrWatching = false;
		rewardBasedVideo = RewardBasedVideoAd.Instance;
	}

	public void HandleOnAdLeavingApplication (object sender, EventArgs args)
	{
		adIsLoadingOrWatching = false;
		rewardBasedVideo = RewardBasedVideoAd.Instance;
	}


	void enableButton (bool enable = true, string disabledText = "NO_ADS")
	{
		if (adButton == null)
			Debug.Log ("enable null");
		else {
			adButton.interactable = enable;
			if (enable)
				adText.text = LocalizationManager.Instance.GetText ("WATCH_AD");
			else
				adText.text = LocalizationManager.Instance.GetText (disabledText);
		}
	}

	void OnDestroy ()
	{
		rewardBasedVideo.OnAdLoaded -= HandleOnAdLoaded;
		rewardBasedVideo.OnAdFailedToLoad -= HandleOnAdFailedToLoad;
		rewardBasedVideo.OnAdOpening -= HandleOnAdOpening;
		rewardBasedVideo.OnAdStarted -= HandleOnAdStarted;
		rewardBasedVideo.OnAdRewarded -= HandleOnAdRewarded;
		rewardBasedVideo.OnAdClosed -= HandleOnAdClosed;
		rewardBasedVideo.OnAdLeavingApplication -= HandleOnAdLeavingApplication;
	}

	void OnEnable ()
	{
		if (isStarted) {
			if (rewardBasedVideo.IsLoaded ())
				enableButton (true);
			else
				enableButton (false);
		}
	}
}
