using System;
using System.Collections.Generic;
using System.IO;

namespace GodotResourceUID
{
    public class ResourceUID
    {
        public const long INVALID_ID = -1;
        
        // Character mapping table, consistent with C++ implementation
        private static readonly char[] uuid_characters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', '0', '1', '2', '3', '4', '5', '6', '7', '8' };
        private static readonly uint uuid_characters_element_count = (uint)uuid_characters.Length;
        private const uint char_count = (uint)('z' - 'a');
        private const uint base_value = char_count + (uint)('9' - '0');
        
        // Internal cache structure
        private readonly Dictionary<long, string> unique_ids = new Dictionary<long, string>();
        private readonly Dictionary<string, long> reverse_cache = new Dictionary<string, long>();
        
        /// <summary>
        /// Convert ID to text format (e.g., uid://d4n4ub6itg400)
        /// </summary>
        /// <param name="id">UID</param>
        /// <returns>Text format of UID</returns>
        public static string IdToText(long id)
        {
            if (id < 0)
            {
                return "uid://<invalid>";
            }
            
            char[] tmp = new char[13]; // Maximum 13 characters
            uint tmp_size = 0;
            long temp_id = id;
            
            do
            {
                uint c = (uint)(temp_id % uuid_characters_element_count);
                tmp[tmp_size] = uuid_characters[c];
                temp_id /= uuid_characters_element_count;
                ++tmp_size;
            } while (temp_id > 0);
            
            // Build full UID string
            char[] result = new char[6 + tmp_size]; // uid:// + actual ID characters
            result[0] = 'u';
            result[1] = 'i';
            result[2] = 'd';
            result[3] = ':';
            result[4] = '/';
            result[5] = '/';
            
            // Reverse character order because the above loop generates the number backward
            for (uint i = 0; i < tmp_size; ++i)
            {
                result[6 + i] = tmp[tmp_size - i - 1];
            }
            
            return new string(result);
        }
        
        /// <summary>
        /// Convert text format UID to ID
        /// </summary>
        /// <param name="text">Text format UID</param>
        /// <returns>UID</returns>
        public static long TextToId(string text)
        {
            if (!text.StartsWith("uid://") || text == "uid://<invalid>")
            {
                return INVALID_ID;
            }
            
            long uid = 0;
            for (int i = 6; i < text.Length; i++)
            {
                uid *= base_value;
                char c = text[i];
                if (char.IsLower(c))
                {
                    uid += c - 'a';
                }
                else if (char.IsDigit(c))
                {
                    uid += c - '0' + char_count;
                }
                else
                {
                    return INVALID_ID;
                }
            }
            
            return uid & 0x7FFFFFFFFFFFFFFF; // Ensure it's positive
        }
        
        /// <summary>
        /// Load UID mapping from cache file
        /// </summary>
        /// <param name="cacheFilePath">Cache file path</param>
        /// <returns>Whether loading was successful</returns>
        public bool LoadFromCache(string cacheFilePath)
        {
            try
            {
                if (!File.Exists(cacheFilePath))
                {
                    return false;
                }
                
                using (BinaryReader reader = new BinaryReader(File.OpenRead(cacheFilePath)))
                {
                    uint entry_count = reader.ReadUInt32();
                    
                    for (uint i = 0; i < entry_count; i++)
                    {
                        long id = reader.ReadInt64();
                        int len = reader.ReadInt32();
                        byte[] buffer = reader.ReadBytes(len);
                        string path = System.Text.Encoding.UTF8.GetString(buffer);
                        
                        unique_ids[id] = path;
                        reverse_cache[path] = id;
                    }
                }
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        /// <summary>
        /// Get path from cache file directly by UID text, without loading the entire cache
        /// </summary>
        /// <param name="cacheFilePath">Cache file path</param>
        /// <param name="uidString">UID text</param>
        /// <returns>Corresponding path, empty string if not found</returns>
        public static string GetPathFromCache(string cacheFilePath, string uidString)
        {
            long uid = TextToId(uidString);
            if (uid == INVALID_ID)
            {
                return string.Empty;
            }
            
            try
            {
                if (!File.Exists(cacheFilePath))
                {
                    return string.Empty;
                }
                
                using (BinaryReader reader = new BinaryReader(File.OpenRead(cacheFilePath)))
                {
                    uint entry_count = reader.ReadUInt32();
                    
                    for (uint i = 0; i < entry_count; i++)
                    {
                        long id = reader.ReadInt64();
                        int len = reader.ReadInt32();
                        byte[] buffer = reader.ReadBytes(len);
                        
                        if (id == uid)
                        {
                            return System.Text.Encoding.UTF8.GetString(buffer);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Ignore exceptions, return empty string
            }
            
            return string.Empty;
        }
        
        /// <summary>
        /// Get path by ID
        /// </summary>
        /// <param name="id">UID</param>
        /// <returns>Corresponding path, empty string if not found</returns>
        public string GetIdPath(long id)
        {
            if (id == INVALID_ID)
            {
                return string.Empty;
            }
            
            if (unique_ids.TryGetValue(id, out string path))
            {
                return path;
            }
            
            return string.Empty;
        }
        
        /// <summary>
        /// Get ID by path
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>Corresponding UID, INVALID_ID if not found</returns>
        public long GetPathId(string path)
        {
            if (reverse_cache.TryGetValue(path, out long id))
            {
                return id;
            }
            
            return INVALID_ID;
        }
        
        /// <summary>
        /// Convert UID text to path
        /// </summary>
        /// <param name="uid">UID text</param>
        /// <returns>Corresponding path, empty string if not found</returns>
        public string UidToPath(string uid)
        {
            long id = TextToId(uid);
            return GetIdPath(id);
        }
        
        /// <summary>
        /// Ensure returns a path, convert to path if input is UID
        /// </summary>
        /// <param name="pathOrUid">Path or UID text</param>
        /// <returns>Path</returns>
        public string EnsurePath(string pathOrUid)
        {
            if (pathOrUid.StartsWith("uid://"))
            {
                return UidToPath(pathOrUid);
            }
            return pathOrUid;
        }
    }
}
