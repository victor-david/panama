using System;
using System.IO;
using System.Text;

//namespace Restless.App.Panama
//{
//    /// <summary>
//    /// The references in Visual Studio to Interop.ADODB and Interop.CDO must be marked as follows:
//    ///   - Embed Interop Types: false // if not, you will see compile error 'CDO.MessageClass' has no constructors defined'
//    ///   - Copy to local: true // needs to be in the bin folder
//    ///  
//    /// You can manually create an interop DLL by pointing Visual Studio to the CDO and ADODB dll's
//    /// 
//    /// The interop DLL's included here have the benefit that they are signed with a strong name. 
//    /// This helps if you need to use them from a signed assembly
//    /// Signing was done by disassembling the generated interop DLL's, signing them and re-compiling them with ILASM
//    /// </summary>
//    public static class CDOParser
//    {
//        /// <summary>
//        /// Loads a message from an EML file.
//        /// </summary>
//        /// <param name="emlFilePath">The full path and file name of the EML file</param>
//        /// <returns>The message object</returns>
//        public static CDO.Message LoadMessage(string emlFilePath)
//        {
//            try
//            {
//                CDO.Message msg = new CDO.MessageClass();
//                ADODB.Stream stream = new ADODB.StreamClass();

//                stream.Open(Type.Missing, ADODB.ConnectModeEnum.adModeUnknown, ADODB.StreamOpenOptionsEnum.adOpenStreamUnspecified, String.Empty, String.Empty);
//                //http://stackoverflow.com/questions/936422/recommendations-on-parsing-eml-files-in-c-sharp
//                //stream.Type = ADODB.StreamTypeEnum.adTypeBinary; // Don't parse UTF8 byte headers
//                stream.LoadFromFile(emlFilePath);
//                stream.Flush();
//                msg.DataSource.OpenObject(stream, "_Stream");
//                msg.DataSource.Save();
//                return msg;
//            }
//            catch (Exception ex)
//            {
//                throw new ApplicationException(string.Format("Could not load message '{0}'. {1}", emlFilePath, ex.Message), innerException: ex);
//            }
//        }

//        public static string GetString(this CDO.IBodyPart bodypart)
//        {
//            var bodyBytes = bodypart.GetDecodedContentStream().ToByteArray();
//            if (string.IsNullOrEmpty(bodypart.Charset))
//            {
//                bodypart.Charset = "utf-8";
//            }

//            var bodyPartCharset = Encoding.GetEncoding(bodypart.Charset);
//            var byteArrayEncoding = Encoding.Unicode; // all UTF-x formats are saved in UTF-16 by ADODB
//            if (bodyPartCharset.IsSingleByte)
//            {
//                byteArrayEncoding = bodyPartCharset; // US-ASCII
//            }

//            var result = byteArrayEncoding.GetString(bodyBytes);
//            return result;
//        }

//        private static byte[] ToByteArray(this ADODB.Stream stream)
//        {
//            byte[] data;

//            var tempSubFolderPath = TempSubfolder();
//            try
//            {
//                var tempFilePath = Path.Combine(tempSubFolderPath, "stream.tmp");
//                // ADODB cannot convert streams to .net byte arrays, but this works
//                stream.SaveToFile(tempFilePath);
//                data = File.ReadAllBytes(tempFilePath);
//            }
//            finally
//            {
//                new DirectoryInfo(tempSubFolderPath).Delete(recursive: true);
//            }

//            return data;
//        }

//        private static byte[] ToByteArray(this CDO.IBodyPart attachment)
//        {
//            byte[] data;

//            var tempSubFolderPath = TempSubfolder();
//            try
//            {
//                var tempFilePath = Path.Combine(tempSubFolderPath, "attachment.tmp");
//                // ADODB cannot convert streams to .net byte arrays, but this works
//                attachment.SaveToFile(tempFilePath);
//                data = File.ReadAllBytes(tempFilePath);
//            }
//            finally
//            {
//                new DirectoryInfo(tempSubFolderPath).Delete(recursive: true);
//            }

//            return data;
//        }

//        /// <summary>
//        /// Returns a random new sub folder of SystemTemp, like %TEMP%\12323
//        /// </summary>
//        private static string TempSubfolder()
//        {
//            var subFolderName = Guid.NewGuid().ToString().Replace("-", "");
//            var systemTempFolderPath = Environment.GetEnvironmentVariable("TEMP");
//            var folderPath = Path.Combine(systemTempFolderPath, subFolderName);
//            Directory.CreateDirectory(folderPath);
//            return folderPath;
//        }
//    }
//}

