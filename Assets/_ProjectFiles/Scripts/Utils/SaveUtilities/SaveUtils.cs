using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Sirenix.Serialization;

namespace Gasanov.Utils.SaveUtilities
 {
     public static class SaveUtils
     {
         /// <summary>
         /// Расширение файла для сохраняемых данных
         /// </summary>
         private const string SaveExtension = ".txt";

         /// <summary>
         /// Папка сохраняемых данных. Оканчивается на "/"
         /// </summary>
         private static readonly string SaveFolder = Application.dataPath + "/Storage/";

         /// <summary>
         /// Инициализирована ли сейчас система сохранений
         /// </summary>
         private static bool _isInitialized;


         private static void Initialize()
         {
             if (_isInitialized == false)
             {
                 _isInitialized = true;

                 // Проверка существования папки с сохранениями
                 if (Directory.Exists(SaveFolder))
                 {
                     // Создание отсутствующей папки с сохранениями
                     Directory.CreateDirectory(SaveFolder);
                 }
             }
         }

         /// <summary>
         /// Сохранение данных 
         /// </summary>
         /// <param name="fileName">Название сохраняемого файла без расширения</param>
         /// <param name="saveString">Сохраняемые данные</param>
         /// <param name="overwrite">Нужно ли перезаписать файл?</param>
         /// <param name="additionalPath">Дополнительный путь каталогов "xxx/xxx/"</param>
         public static void SaveDataText(string fileName, string saveString, bool overwrite, string additionalPath = "")
         {
             Initialize();

             var saveFileName = fileName;

             // Если нельзя переписать файл
             if (overwrite == false)
             {
                 // Уникальный номер файла
                 int saveNumber = 0;
                 while (File.Exists(SaveFolder + additionalPath + saveFileName + SaveExtension))
                 {
                     // Увеличиваем уникальный номер, если такой файл уже существует
                     saveNumber++;
                     // Приписываем номер к названию
                     saveFileName = fileName + "_" + saveNumber;
                 }
             }

             // Запись в файл
             File.WriteAllText(SaveFolder + additionalPath + saveFileName + SaveExtension, saveString);
         }

         /// <summary>
         /// Сохранение данных 
         /// </summary>
         /// <param name="fileName">Название сохраняемого файла без расширения</param>
         /// <param name="saveBytes">Сохраняемые данные</param>
         /// <param name="overwrite">Нужно ли перезаписать файл?</param>
         /// <param name="additionalPath">Дополнительный путь каталогов "xxx/xxx/"</param>
         public static void SaveDataBytes(string fileName, byte[] saveBytes, bool overwrite, string additionalPath = "")
         {
             Initialize();

             var saveFileName = fileName;

             // Если нельзя переписать файл
             if (overwrite == false)
             {
                 // Уникальный номер файла
                 int saveNumber = 0;
                 while (File.Exists(SaveFolder + additionalPath + saveFileName + SaveExtension))
                 {
                     // Увеличиваем уникальный номер, если такой файл уже существует
                     saveNumber++;
                     // Приписываем номер к названию
                     saveFileName = fileName + "_" + saveNumber;
                 }
             }

             if (!Directory.Exists(SaveFolder + additionalPath))
                 Directory.CreateDirectory(SaveFolder + additionalPath);
             // Запись в файл
             File.WriteAllBytes(SaveFolder + additionalPath + saveFileName + SaveExtension, saveBytes);
         }

         /// <summary>
         /// Загрузка данных из файла
         /// </summary>
         /// <param name="fileName">Название файла</param>
         /// <param name="additionalPath">Дополнительный путь каталогов "xxx/xxx/"</param>
         public static string LoadDataText(string fileName, string additionalPath = "")
         {
             Initialize();

             // Если файл существует
             if (File.Exists(SaveFolder + additionalPath + fileName + SaveExtension))
             {
                 // Данные из файла
                 string saveString = File.ReadAllText(SaveFolder + additionalPath + fileName + SaveExtension);
                 return saveString;
             }
             else
             {
                 return null;
             }
         }

         /// <summary>
         /// Загрузка данных из файла
         /// </summary>
         /// <param name="fileName">Название файла</param>
         /// <param name="additionalPath">Дополнительный путь каталогов "xxx/xxx/"</param>
         public static byte[] LoadDataBytes(string fileName, string additionalPath = "")
         {
             Initialize();

             // Если файл существует
             if (File.Exists(SaveFolder + additionalPath + fileName + SaveExtension))
             {
                 // Данные из файла
                 byte[] saveBytes = File.ReadAllBytes(SaveFolder + additionalPath + fileName + SaveExtension);
                 return saveBytes;
             }
             else
             {
                 return null;
             }
         }

         /// <summary>
         /// Загрузка данных из последнего измененного файла
         /// </summary>
         /// <param name="fileName">Название файла</param>
         /// <param name="additionalPath">Дополнительный путь каталогов "xxx/xxx/"</param>
         public static string LoadMostRecentData(string fileName, string additionalPath = "")
         {
             DirectoryInfo directoryInfo = new DirectoryInfo(SaveFolder + additionalPath);

             FileInfo[] files = directoryInfo.GetFiles("*" + SaveExtension);

             // Файл с самым поздним временем изменения 
             FileInfo mostRecentFile = null;

             // Проходимся по всем файлам
             foreach (var fileInfo in files)
             {
                 if (mostRecentFile == null)
                 {
                     mostRecentFile = fileInfo;
                 }
                 else
                 {
                     // Если последняя запись в fileInfo была сделана позже
                     if (fileInfo.LastWriteTime > mostRecentFile.LastWriteTime)
                     {
                         mostRecentFile = fileInfo;
                     }
                 }
             }

             // Если файл был найден
             if (mostRecentFile != null)
             {
                 string saveString = File.ReadAllText(mostRecentFile.FullName);
                 return saveString;
             }
             else
             {
                 return null;
             }
         }


         public static string ConvertObject<TObject>(TObject serializableObject)
         {
             var json = SerializationUtility.SerializeValue<TObject>(serializableObject, DataFormat.JSON);

             return Encoding.UTF8.GetString(json);
         }

         /// <summary>
         /// Сохраняет объект указанного типа в файл
         /// </summary>
         /// <param name="saveObject"></param>
         /// <param name="fileName"></param>
         /// <param name="overwrite"></param>
         /// <param name="additionalPath"></param>
         public static void SaveObject<TObject>(TObject saveObject, string fileName, bool overwrite,
             string additionalPath = "")
         {
             var odinJson = SerializationUtility.SerializeValue<TObject>(saveObject, DataFormat.JSON);

             SaveDataBytes(fileName, odinJson, overwrite, additionalPath);
         }

         /// <summary>
         /// Загружает объект указанного типа из файла
         /// </summary>
         /// <param name="fileName"></param>
         /// <param name="additionalPath"></param>
         /// <typeparam name="TObject"></typeparam>
         /// <returns></returns>
         public static TObject LoadObject<TObject>(string fileName, string additionalPath = "")
         {
             var odinJsonBytes = LoadDataBytes(fileName, additionalPath);

             var loadedObject = SerializationUtility.DeserializeValue<TObject>(odinJsonBytes, DataFormat.JSON);

             if (loadedObject == null) return default(TObject);

             return loadedObject;
         }

         /// <summary>
         /// Сохраняет значение в файл.
         /// </summary>
         /// <param name="fileName">Название файла</param>
         /// <param name="additionalPath">Путь к файлу</param>
         /// <param name="property">Значение</param>
         /// <param name="propertyName">Название значения</param>
         /// <param name="overwrite"></param>
         public static void SaveProperty(string fileName, string additionalPath,
             string property, string propertyName, bool overwrite = true)
         {
             Initialize();

             if (!Directory.Exists(SaveFolder + additionalPath))
                 Directory.CreateDirectory(SaveFolder + additionalPath);

             if (!File.Exists(SaveFolder + additionalPath + fileName + SaveExtension))
                 File.Create(SaveFolder + additionalPath + fileName + SaveExtension).Close();

             // basic pattern for properties ((chance)\s*:\s*([-+]?[0-9]*\.?[0-9]+)\s*;)
             var regex = new Regex($"({propertyName})\\s*:\\s*([-+]?[0-9]*\\,?[0-9]+)\\s*;");

             var fileData = File.ReadAllText(SaveFolder + additionalPath + fileName + SaveExtension);

             if (overwrite)
             {
                 // Находим значение с таким же названием и заменяем значение
                 var match = regex.Match(fileData);
                 if (match.Success)
                 {
                     fileData.Remove(match.Index, match.Length);
                     fileData.Insert(match.Index, $"{propertyName}:{property}");
                 }
                 else
                 {
                     fileData += $"{propertyName}:{property};";
                 }
             }
             else
             {
                 fileData += $"{propertyName}:{property};";
             }

             File.WriteAllText(SaveFolder + additionalPath + fileName + SaveExtension, fileData);
         }

         /// <summary>
         /// Загружает значение из указанного файла. В случае провала загрузки Match будет иметь свойство Success == false
         /// </summary>
         /// <param name="fileName">Название файла</param>
         /// <param name="additionalPath">Путь к файлу</param>
         /// <param name="regexPattern">Шаблон выражения</param>
         /// <returns></returns>
         public static T LoadProperty<T>(string fileName, string additionalPath, string propertyName,
             bool createDefault = false)
         {
             // Если файла не существует
             if (!File.Exists(SaveFolder + additionalPath + fileName + SaveExtension))
             {
                 // Создаем пустышку
                 if (createDefault)
                     SaveProperty(fileName, additionalPath, default(T).ToString(),
                         propertyName, true);

                 // Возвращаем результат провала загрузки
                 return default(T);
             }
             else
             {
                 var fileData = File.ReadAllText(SaveFolder + additionalPath + fileName + SaveExtension);

                 var match = Regex.Match(fileData,
                     $"({propertyName})\\s*:\\s*([-+]?[0-9]*[\\,.]?[0-9]+)\\s*;");

                 // Проверка успеха поиска
                 if (match.Success)
                 {
                     return ChangeType<T>(match.Groups[2].Value.Replace('.', ','));
                 }
                 else
                 {
                     // Создаем пустышку
                     if (createDefault)
                         SaveProperty(fileName, additionalPath, default(T).ToString(),
                             propertyName, true);

                     return default(T);
                 }
             }
         }

         public static T ChangeType<T>(this object obj)
         {
             return (T) Convert.ChangeType(obj, typeof(T));
         }

         /// <summary>
         /// Возвращает прокси для файла со свойствами.
         /// При отсутствии файла создает новый, если createDefault == true.
         /// </summary>
         public static PropertyFileProxy GetProxy(string filePath, bool createDefault = true)
         {
             var path = SaveFolder + filePath + SaveExtension;
             Directory.CreateDirectory(Path.GetDirectoryName(path));

             if (File.Exists(path))
             {
                 var fs = File.Open(path,
                     FileMode.Open, FileAccess.ReadWrite);

                 return new PropertyFileProxy(fs);
             }

             if (createDefault)
             {
                 var fs = new FileStream(path, FileMode.Create,
                     FileAccess.ReadWrite);

                 return new PropertyFileProxy(fs);
             }

             throw new FileNotFoundException();
         }

     }

     [System.Serializable]
    public class TestData
    {
        public TestData()
        {

        }

        public TestData(int count, string name, int Count)
        {
            this.count = count;
            this.name = name;
            this.Count = Count;
        }

        private int count;

        public string name;

        public int Count { get; private set; }
    }
}




