using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class BaseGameProfileCharacterAttributes {    

    public static string ATT_CHARACTER_CODE = "att-character-code";
    public static string ATT_CHARACTER_COSTUME_CODE = "att-character-costume-code";
    public static string ATT_CHARACTERS = "att-characters";
}

public class BaseGameProfileCharacters {
    private static volatile BaseGameProfileCharacter current;
    private static volatile BaseGameProfileCharacters instance;
    private static object syncRoot = new Object();
    public static string DEFAULT_USERNAME = "Player";
 
    public static BaseGameProfileCharacter BaseCurrent {
        get {
            if(current == null) {
                lock(syncRoot) {
                    if(current == null) 
                        current = new BaseGameProfileCharacter();
                }
            }
 
            return current;
        }
        set {
            current = value;
        }
    }
     
    public static BaseGameProfileCharacters BaseInstance {
        get {
            if(instance == null) {
                lock(syncRoot) {
                    if(instance == null) 
                        instance = new BaseGameProfileCharacters();
                }
            }
 
            return instance;
        }
    }
 
    // TODO: Common profile actions, lookup, count, etc

    public static GameProfilePlayerProgressItem currentProgress {
        get {
            return GameProfileCharacters.Current.GetCurrentCharacter().profilePlayerProgress;
        }
    }

    public static GameProfileRPGItem currentRPG {
        get {
            return GameProfileCharacters.Current.GetCurrentCharacter().profileRPGItem;
        }
    }

    public static GameProfileCustomItem currentCustom {
        get {
            return GameProfileCharacters.Current.GetCurrentCharacter().profileCustomItem;
        }
    }

    public static GameProfileCharacterItem currentCharacter = null;
}

public class GameProfileCharacterItems {
 
    public List<GameProfileCharacterItem> items;
 
    public GameProfileCharacterItems() {
        Reset();
    }
 
    public void Reset() {
        items = new List<GameProfileCharacterItem>();
    }
 
    public GameProfileCharacterItem GetCharacter(string code) {
        //if(items == null) {
        //     items = new List<GameProfileCharacterItem>();
        //
        //    GameProfileCharacterItem item = new GameProfileCharacterItem();
        //    items.Add(item);
        //}

        if(items == null) {
            return null;
        }

        foreach(GameProfileCharacterItem item in items) {
            if(item.code.ToLower() == code.ToLower()) {
                return item;
            }
        }
        return null;
    }
 
    public void SetCharacter(string code, GameProfileCharacterItem item) {
        bool found = false;
     
        for(int i = 0; i < items.Count; i++) {
            if(items[i].code.ToLower() == code.ToLower()) {
                items[i] = item;
                found = true;
                break;
            }
        }
     
        if(!found) {
            items.Add(item);
        }
    }

    public void SetCharacterRPG(string code, GameProfileRPGItem item) {
        GameProfileCharacterItem character = GetCharacter(code);

        if(character == null) {
            character = new GameProfileCharacterItem();
        }

        character.profileRPGItem = item;
        SetCharacter(code, character);
    }

    public void SetCharacterCustom(string code, GameProfileCustomItem item) {
        GameProfileCharacterItem character = GetCharacter(code);

        if(character == null) {
            character = new GameProfileCharacterItem();
        }

        character.profileCustomItem = item;
        SetCharacter(code, character);
    }

    public void SetCharacterProgress(string code, GameProfilePlayerProgressItem item) {
        GameProfileCharacterItem character = GetCharacter(code);

        if(character == null) {
            character = new GameProfileCharacterItem();
        }

        character.profilePlayerProgress = item;
        SetCharacter(code, character);
    }
}

public class GameProfileCharacterItem : DataObject {
 
    public bool current = true;
    public string code = "default";
    public string characterCode = "default";
    public string characterCostumeCode = "default";
    public GameProfileRPGItem profileRPGItem;
    public GameProfilePlayerProgressItem profilePlayerProgress;
    public GameProfileCustomItem profileCustomItem;

    public GameProfileCharacterItem() {
        Reset();
    }
    
    public override void Reset() {
        base.Reset();

        current = true;
        code = "default";
        characterCode = "default";
        characterCostumeCode = "default";
        profileRPGItem = new GameProfileRPGItem();
        profilePlayerProgress = new GameProfilePlayerProgressItem();
        profileCustomItem = new GameProfileCustomItem();

    }
}

public class BaseGameProfileCharacter : Profile {
    // BE CAREFUL adding properties as they will cause a need for a profile conversion
    // Best way to add items to the profile is the GetAttribute and SetAttribute class as 
    // that stores as a generic DataAttribute class.  Booleans, strings, objects, serialized json objects etc
    // all work well and cause no need to convert profile on updates. 
     
    public BaseGameProfileCharacter() {
        Reset();
    }
 
    public override void Reset() {
        base.Reset();
        username = ProfileConfigs.defaultPlayerName;
    }
 
    // characters
 
    public virtual void SetCharacters(GameProfileCharacterItems obj) {
        string dataText = JsonMapper.ToJson(obj);
        //LogUtil.Log("SetCharacters: " + dataText);
        SetAttributeStringValue(BaseGameProfileCharacterAttributes.ATT_CHARACTERS, dataText);

        Messenger.Broadcast(BaseGameProfileMessages.ProfileShouldBeSaved);
    }

    public virtual GameProfileCharacterItems GetCharacters() {       
        GameProfileCharacterItems obj = new GameProfileCharacterItems();

        string key = BaseGameProfileCharacterAttributes.ATT_CHARACTERS;

        if(!CheckIfAttributeExists(key)) {

            SetCharacters(obj);
        }

        string json = GetAttributeStringValue(key);
     
        if(!string.IsNullOrEmpty(json)) {
         
            try {
                //LogUtil.Log("GetCharacters: " + json);
                obj = JsonMapper.ToObject<GameProfileCharacterItems>(json);
            }
            catch(Exception e) {
                obj = new GameProfileCharacterItems();
                LogUtil.Log(e);
            }
        }
     
        if(obj.items.Count == 0) {
            // add default
            obj.SetCharacter("default", new GameProfileCharacterItem());
        }        
     
        return obj;
    }

    // helpers

    public void CurrentCharacterAddGamePlayerProgressXP(double val) {
        GameProfileCharacterItem character = GetCurrentCharacter();
        character.profilePlayerProgress.AddGamePlayerProgressXP(val);
        SetCharacter(character);
    }

    public void CurrentCharacterAddGamePlayerProgressLevel(double val) {
        GameProfileCharacterItem character = GetCurrentCharacter();
        character.profilePlayerProgress.AddGamePlayerProgressLevel(val);
        SetCharacter(character);
    }

    public void CurrentCharacterAddGamePlayerProgressEnergy(double val) {
        GameProfileCharacterItem character = GetCurrentCharacter();
        character.profilePlayerProgress.AddGamePlayerProgressEnergy(val);
        SetCharacter(character);
    }

    public void CurrentCharacterAddGamePlayerProgressHealth(double val) {
        GameProfileCharacterItem character = GetCurrentCharacter();
        character.profilePlayerProgress.AddGamePlayerProgressHealth(val);
        SetCharacter(character);
    }
 
    // character 
 
    public GameProfileRPGItem GetCurrentCharacterRPG() {
        return GetCurrentCharacter().profileRPGItem;
    }

    public GameProfileCustomItem GetCurrentCharacterCustom() {
        return GetCurrentCharacter().profileCustomItem;
    }

    public GameProfilePlayerProgressItem GetCurrentCharacterProgress() {
        return GetCurrentCharacter().profilePlayerProgress;
    }
 
    public GameProfileCharacterItem GetCurrentCharacter() {
        return GetCharacter("default");
    }
 
    public GameProfileRPGItem GetCharacterRPG(string code) {
        return GetCharacter(code).profileRPGItem;
    }

    public GameProfileCustomItem GetCharacterCustom(string code) {
        return GetCharacter(code).profileCustomItem;
    }

    public GameProfilePlayerProgressItem GetCharacterProgress(string code) {
        return GetCharacter(code).profilePlayerProgress;
    }
 
    public GameProfileCharacterItem GetCharacter(string code) {

        if(BaseGameProfileCharacters.currentCharacter == null) {
            GameProfileCharacterItem item = GetCharacters().GetCharacter(code);
    
            //if(item == null) {
            //   item = new GameProfileCharacterItem();
            //   GetCharacters().SetCharacter(code, item);
            //}
            if(item != null) {
                item.profileCustomItem = GameCustomController.Instance.CheckCustomColorInit(item.profileCustomItem);
                BaseGameProfileCharacters.currentCharacter = item;
            }
        }

        return BaseGameProfileCharacters.currentCharacter;
    }
 
    public void SetCharacter(string code, GameProfileCharacterItem item) {
        GameProfileCharacterItems characters = GetCharacters();
        BaseGameProfileCharacters.currentCharacter = item;
        characters.SetCharacter(code, item);
        SetCharacters(characters);
    }

    public void SetCharacter(GameProfileCharacterItem item) {
        SetCharacter("default", item);
    }

    public void SetCharacterRPG(string code, GameProfileRPGItem item) {
        GameProfileCharacterItems characters = GetCharacters();
        characters.SetCharacterRPG(code, item);
        SetCharacters(characters);
    }

    public void SetCharacterProgress(string code, GameProfilePlayerProgressItem item) {
        GameProfileCharacterItems characters = GetCharacters();
        characters.SetCharacterProgress(code, item);
        SetCharacters(characters);
    }

    public void SetCharacterCustom(string code, GameProfileCustomItem item) {
        GameProfileCharacterItems characters = GetCharacters();
        characters.SetCharacterCustom(code, item);
        SetCharacters(characters);
    }

    public void SetCharacterRPG(GameProfileRPGItem item) {
        SetCharacterRPG("default", item);
    }

    public void SetCharacterCustom(GameProfileCustomItem item) {
        SetCharacterCustom("default", item);
    }
     
    // customizations        
 
    public virtual void SetValue(string code, object value) {
        DataAttribute att = new DataAttribute();
        att.val = value;
        att.code = code;
        att.name = "";
        att.type = "bool";
        att.otype = "character";
        SetAttribute(att);
    }
 
    public virtual bool GetValue(string code) {
        bool currentValue = false;
        object objectValue = GetAttribute(code).val;
        if(objectValue != null) {
            currentValue = Convert.ToBoolean(objectValue);
        }
     
        return currentValue;
    }
 
    public virtual List<DataAttribute> GetList() {
        return GetAttributesList("character");
    }    
 
    // CHARACTER - Player specific
 
 
    public string GetCurrentCharacterCode() {
        return GetCurrentCharacterCode("default");
    }
 
    public string GetCurrentCharacterCode(string defaultValue) {
        string attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileCharacterAttributes.ATT_CHARACTER_CODE))
            attValue = GetAttributeStringValue(BaseGameProfileCharacterAttributes.ATT_CHARACTER_CODE);
        return attValue;
    }
 
    public void SetCurrentCharacterCode(string attValue) {
        SetAttributeStringValue(BaseGameProfileCharacterAttributes.ATT_CHARACTER_CODE, attValue);
    }
 
    public string GetCurrentCharacterCostumeCode() {
        return GetCurrentCharacterCostumeCode("default");
    }
 
    public string GetCurrentCharacterCostumeCode(string defaultValue) {
        string attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileCharacterAttributes.ATT_CHARACTER_COSTUME_CODE))
            attValue = GetAttributeStringValue(BaseGameProfileCharacterAttributes.ATT_CHARACTER_COSTUME_CODE);
        return attValue;
    }
 
    public void SetCurrentCharacterCostumeCode(string attValue) {
        SetAttributeStringValue(BaseGameProfileCharacterAttributes.ATT_CHARACTER_COSTUME_CODE, attValue);
    }        
}
