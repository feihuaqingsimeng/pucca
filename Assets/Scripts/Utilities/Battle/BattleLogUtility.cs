using NAMU;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class BattleLogUtility
{
    public static bool TryParse(BattleLog battleLog, out string value)
    {
        if (battleLog != null)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(memoryStream, battleLog);
                byte[] serializedBytes = memoryStream.ToArray();
                if (serializedBytes != null
                    && serializedBytes.Length > 0)
                {
                    string deserializedString = Convert.ToBase64String(serializedBytes);
                    //string compressedString = SimpleCompressor.CompressString(serializedString);

                    value = SimpleCompressor.CompressString(deserializedString);

                    return true;
                }
            }
        }

        value = string.Empty;

        return false;
    }

    public static bool TryGetBattleLog(string value, out BattleLog battleLog)
    {
        if (!string.IsNullOrEmpty(value))
        {
            string decompressedString = SimpleCompressor.DecompressString(value);
            byte[] deserializedBytes = Convert.FromBase64String(decompressedString);
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using (MemoryStream memoryStream = new MemoryStream(deserializedBytes, false))
            {
                battleLog = binaryFormatter.Deserialize(memoryStream) as BattleLog;

                return (battleLog != null);
            }
        }

        battleLog = null;

        return false;
    }
}
