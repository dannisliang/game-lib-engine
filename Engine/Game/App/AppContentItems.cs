using System;
using System.Collections.Generic;
using System.IO;

public class AppContentItems : BaseAppContentItems<AppContentItem>
{
	private static volatile AppContentItem current;
	private static volatile AppContentItems instance;
	private static System.Object syncRoot = new System.Object();
	/*
	public static string APP_STATE_BOOKS = "app-state-books";
	public static string APP_STATE_CARDS = "app-state-cards";
	public static string APP_STATE_GAMES = "app-state-games";
	public static string APP_STATE_SETTINGS = "app-state-settings";
	public static string APP_STATE_TROPHIES = "app-state-trophies";
	*/
	
	public static string DATA_KEY = "app-content-item-data";
		
	public static AppContentItem Current {
		get  {
			if (current == null) {
				lock (syncRoot)  {
				   if (current == null) 
				      current = new AppContentItem();
				}
			}
			
			return current;
		}
		set {
			current = value;
		}
	}
		
	public static AppContentItems Instance {
		get  {
			if (instance == null) {
				lock (syncRoot)  {
				   if (instance == null) 
				      instance = new AppContentItems(true);
				}
			}
			
			return instance;
		}
		set {
			instance = value;
		}
	}
			
	public AppContentItems() {
		Reset();
		//ChangeState(APP_STATE_BOOKS);
	}
	
	public AppContentItems(bool loadData) {
		Reset();
		path = "data/" + DATA_KEY + ".json";
		pathKey = DATA_KEY;
		LoadData();
	}
	
	public void ChangeState(string code) {
		if(Current.code != code) {
			Current = GetById(code);
		}
	}
	
	/*
	public List<AppContentItem> GetListByPack(string packCode) {
		List<AppContentItem> filteredList = new List<AppContentItem>();
		foreach(AppContentItem obj in GetAll()) {
			if(packCode.ToLower() == obj.pack_code.ToLower()) {
				filteredList.Add(obj);
			}
		}
		
		return filteredList;
	}
	*/
	
	public List<AppContentItem> GetListByCodeAndPackCode(string assetCode, string packCode) {
		List<AppContentItem> filteredList = new List<AppContentItem>();
		foreach(AppContentItem obj in GetListByPack(packCode)) {
			if(assetCode.ToLower() == obj.code.ToLower()) {
				filteredList.Add(obj);
			}
		}
		
		return filteredList;
	}
	
	public string GetAppContentItemContentPath(string packCode, string asset, bool versioned) {
		string packPath = PathUtil.Combine(
			Contents.appCachePathSharedPacks,
			packCode);
		string packPathContent = PathUtil.Combine(
			packPath, 
			ContentConfig.currentContentContent);
		
		string file = "";
		AppContentItem AppContentItem = AppContentItems.Instance.GetById(asset);
		if(AppContentItem != null) {
			file = AppContentItem.code 
				+ "." 
				+ AppContentItem.GetVersionFileExt();
		}
		
		string fullPath = PathUtil.Combine(packPathContent, file);
		if(versioned) {
			fullPath = Contents.GetFullPathVersioned(fullPath);
		}
		return fullPath;
	}
	
		
	/*
	public void ChangeState(AppState appStateTo) {
		if(lastAppState != appStateTo) {
			appState = appStateTo;
			HandleStateChange();
		}
	}
	
	public void HandleStateChange() {
		if(appState == AppState.StateNotSet) {
			OnAppStateNotStarted();
		}
		else if(appState == AppState.StateBook) {
			OnAppStateBooks();
		}
		else if(appState == AppState.StateCards) {
			OnAppStateCards();
		}
		else if(appState == AppState.StateGames) {
			OnAppStateGames();
		}
	}
	
	public void OnAppStateNotStarted() {
		appState = AppState.StateNotSet;
		appSubState = AppSubState.StateSubEnvironment;
		
		Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateNotSet, appState, appSubState);
	}
	
	public void OnAppStateBooks() {
		appState = AppState.StateBook;
		appSubState = AppSubState.StateSubEnvironment;
			
		currentApp = new App();
		currentApp.appCode = "default";
		
		Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateBooks, appState, appSubState);
	}
	
	public void OnAppStateCards() {
		appState = AppState.StateCards;
		appSubState = AppSubState.StateSubEnvironment;
			
		currentApp = new App();
		currentApp.appCode = "default";
		
		Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateCards, appState, appSubState);
	}
	
	public void OnAppStateGames() {
		appState = AppState.StateCards;
		appSubState = AppSubState.StateSubEnvironment;
			
		currentApp = new App();
		currentApp.appCode = "default";
		
		Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateGames, appState, appSubState);
	}
	*/
}

public class AppContentItemAttributes {
	public static string version_file_increment = "version_file_increment";
	public static string version = "version";
	public static string version_required_app = "version_required_app";
	public static string version_type = "version_type";
	public static string version_file_ext = "version_file_ext";
	public static string version_file_type = "version_file_type";
	/*
	 * 
	    	"version_file_increment":"1",
		    "version":"1.0",
		    "version_required_app":"1.0",
		    "version_type":"itemized"
		    */
}

public class AppContentItemAttributesFileType {
	public static string videoType = "video";
	public static string audioType = "audio";
	public static string imageType = "image";
	public static string assetBundleType = "assetBundle";
}

public class AppContentItemAttributesFileExt {
	public static string videoM4vExt = "m4v";
	public static string videoMp4Ext = "mp4";
	
	public static string audioMp3Ext = "mp3";
	public static string audioWavExt = "wav";
	
	public static string imagePngExt = "png";
	public static string imageJpgExt = "jpg";
	
	public static string assetBundleExt = "unity3d";
}

public class AppContentItem : BaseAppContentItem
{
	
	public List<string> keys;
	public Dictionary<string, string> content_attributes;	
	
	// Attributes that are added or changed after launch should be like this to prevent
	// profile conversions.
			
	public AppContentItem () {
		Reset();
	}
	
	public override void Reset() {
		base.Reset();
		keys = new List<string>();
		content_attributes = new Dictionary<string, string>();
	}
	
	public string GetContentString(string key) {
		string content = "";
		if(content_attributes.ContainsKey(key)) {
			content = content_attributes[key];
		}
		return content;
	}
	
	public string GetVersion() {
		return GetContentString(AppContentItemAttributes.version);
	}
	
	public string GetVersionFileIncrement() {
		return GetContentString(AppContentItemAttributes.version_file_increment);
	}
	
	public string GetVersionRequiredApp() {
		return GetContentString(AppContentItemAttributes.version_required_app);
	}
	
	public string GetVersionType() {
		return GetContentString(AppContentItemAttributes.version_type);
	}	
	
	public string GetVersionFileType() {
		return GetContentString(AppContentItemAttributes.version_file_type);
	}
	
	public string GetVersionFileExt() {
		return GetContentString(AppContentItemAttributes.version_file_ext);
	}
	
}