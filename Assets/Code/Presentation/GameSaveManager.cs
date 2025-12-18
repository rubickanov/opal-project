using System;
using System.Collections.Generic;
using UnityEngine;
using Rubickanov.Opal.Domain;

namespace Rubickanov.Opal.Presentation
{
    public static class GameSaveManager
    {
        private const string SaveKey = "GameSave";

        public static bool HasSave()
        {
            return PlayerPrefs.HasKey(SaveKey);
        }

        public static void DeleteSave()
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
        }

        public static void Save(Game game, Dictionary<int, Color> colors)
        {
            if (game == null)
            {
                return;
            }

            var saveData = new SaveData
            {
                GameSnapshot = game.CreateSnapshot(),
                Colors = SerializeColors(colors)
            };

            string json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        public static bool TryLoad(out Game game, out Dictionary<int, Color> colors)
        {
            game = null;
            colors = new Dictionary<int, Color>();

            if (!HasSave())
            {
                return false;
            }

            try
            {
                string json = PlayerPrefs.GetString(SaveKey);
                var saveData = JsonUtility.FromJson<SaveData>(json);

                game = Game.FromSnapshot(saveData.GameSnapshot);
                colors = DeserializeColors(saveData.Colors);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load save: {e.Message}");
                DeleteSave();
                return false;
            }
        }

        private static List<SerializableColor> SerializeColors(Dictionary<int, Color> colors)
        {
            var result = new List<SerializableColor>();

            foreach (var kvp in colors)
            {
                result.Add(new SerializableColor
                {
                    Key = kvp.Key,
                    R = kvp.Value.r,
                    G = kvp.Value.g,
                    B = kvp.Value.b
                });
            }

            return result;
        }

        private static Dictionary<int, Color> DeserializeColors(List<SerializableColor> colors)
        {
            var result = new Dictionary<int, Color>();

            foreach (var color in colors)
            {
                result[color.Key] = new Color(color.R, color.G, color.B);
            }

            return result;
        }
    }

    [Serializable]
    public class SaveData
    {
        public GameSnapshot GameSnapshot;
        public List<SerializableColor> Colors;
    }

    [Serializable]
    public class SerializableColor
    {
        public int Key;
        public float R;
        public float G;
        public float B;
    }
}
