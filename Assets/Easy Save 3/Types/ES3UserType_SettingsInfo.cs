using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("soundVolume", "musicVolume", "playerStyle")]
	public class ES3UserType_SettingsInfo : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_SettingsInfo() : base(typeof(SettingsInfo)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (SettingsInfo)obj;
			
			writer.WriteProperty("soundVolume", instance.soundVolume, ES3Type_float.Instance);
			writer.WriteProperty("musicVolume", instance.musicVolume, ES3Type_float.Instance);
			writer.WritePropertyByRef("playerStyle", instance.playerStyle);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (SettingsInfo)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "soundVolume":
						instance.soundVolume = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "musicVolume":
						instance.musicVolume = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "playerStyle":
						instance.playerStyle = reader.Read<PlayerStyle>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new SettingsInfo();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_SettingsInfoArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_SettingsInfoArray() : base(typeof(SettingsInfo[]), ES3UserType_SettingsInfo.Instance)
		{
			Instance = this;
		}
	}
}