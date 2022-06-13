using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace BrainFailProductions.PolyFew
{
	public static class SystemServices
	{
		private static void SetPatterns()
		{
			SystemServices.regexPatterns.netError = "<neterror>";
			SystemServices.regexPatterns.nullOrEmpty = "<nullorempty>";
			SystemServices.regexPatterns.generalError = "<generalerror>";
			SystemServices.regexPatterns.apiMistmatch = "<apimismatch>";
			SystemServices.regexPatterns.parametersMismatch = "<parametersmismatch>";
			SystemServices.regexPatterns.nothing = "";
		}

		public static IEnumerator UnityAsyncGETRequest(string encodedUrl, Action<string, long> callback, int? timeout = null, Dictionary<string, string> headers = null)
		{
			SystemServices.SetPatterns();
			UnityWebRequest webRequest = new UnityWebRequest(encodedUrl);
			webRequest.timeout = ((timeout == null) ? webRequest.timeout : timeout.Value);
			webRequest.method = "GET";
			DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
			webRequest.downloadHandler = downloadHandler;
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in headers)
				{
					webRequest.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
				}
			}
			yield return webRequest.SendWebRequest();
			long responseCode = webRequest.responseCode;
			if (webRequest.isHttpError || webRequest.isNetworkError)
			{
				callback("<neterror>" + webRequest.error, responseCode);
			}
			else if (string.IsNullOrEmpty(webRequest.downloadHandler.text))
			{
				callback("<nullorempty>Error! server returned an empty response.", responseCode);
			}
			else
			{
				callback(webRequest.downloadHandler.text, responseCode);
			}
			yield break;
		}

		public static void UnityBlockingGETRequest(string encodedUrl, Action<string, long> callback, int? timeout = null, Dictionary<string, string> headers = null)
		{
			SystemServices.SetPatterns();
			UnityWebRequest unityWebRequest = new UnityWebRequest(encodedUrl);
			unityWebRequest.timeout = ((timeout == null) ? unityWebRequest.timeout : timeout.Value);
			unityWebRequest.method = "GET";
			DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
			unityWebRequest.downloadHandler = downloadHandler;
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in headers)
				{
					unityWebRequest.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
				}
			}
			unityWebRequest.SendWebRequest();
			while (!unityWebRequest.isDone)
			{
			}
			long responseCode = unityWebRequest.responseCode;
			if (unityWebRequest.isHttpError || unityWebRequest.isNetworkError)
			{
				callback("<neterror>" + unityWebRequest.error, responseCode);
				return;
			}
			if (string.IsNullOrEmpty(unityWebRequest.downloadHandler.text))
			{
				callback("<nullorempty>Error! server returned an empty response.", responseCode);
				return;
			}
			callback(unityWebRequest.downloadHandler.text, responseCode);
		}

		public static void UnityBlockingPOSTRequest(string baseUrl, Action<string, long> callback, byte[] data, int? timeout = null, Dictionary<string, string> headers = null)
		{
			SystemServices.SetPatterns();
			UnityWebRequest unityWebRequest = new UnityWebRequest(baseUrl);
			unityWebRequest.timeout = ((timeout == null) ? unityWebRequest.timeout : timeout.Value);
			unityWebRequest.method = "POST";
			UploadHandlerRaw uploadHandler = new UploadHandlerRaw(data);
			DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
			unityWebRequest.uploadHandler = uploadHandler;
			unityWebRequest.downloadHandler = downloadHandler;
			unityWebRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in headers)
				{
					unityWebRequest.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
				}
			}
			unityWebRequest.SendWebRequest();
			while (!unityWebRequest.isDone)
			{
			}
			long responseCode = unityWebRequest.responseCode;
			if (unityWebRequest.isHttpError || unityWebRequest.isNetworkError)
			{
				callback("<neterror>" + unityWebRequest.error, responseCode);
				return;
			}
			if (string.IsNullOrEmpty(unityWebRequest.downloadHandler.text))
			{
				callback("<nullorempty>Error! server returned an empty response.", responseCode);
				return;
			}
			callback(unityWebRequest.downloadHandler.text, responseCode);
		}

		public static IEnumerator UnityAsyncPOSTRequest(string baseUrl, Action<string, long> callback, byte[] data, int? timeout = null, Dictionary<string, string> headers = null)
		{
			SystemServices.SetPatterns();
			UnityWebRequest webRequest = new UnityWebRequest(baseUrl);
			webRequest.timeout = ((timeout == null) ? webRequest.timeout : timeout.Value);
			webRequest.method = "POST";
			UploadHandlerRaw uploadHandler = new UploadHandlerRaw(data);
			DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
			webRequest.uploadHandler = uploadHandler;
			webRequest.downloadHandler = downloadHandler;
			webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in headers)
				{
					webRequest.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
				}
			}
			yield return webRequest.SendWebRequest();
			long responseCode = webRequest.responseCode;
			if (webRequest.isHttpError || webRequest.isNetworkError)
			{
				callback("<neterror>" + webRequest.error, responseCode);
			}
			else if (string.IsNullOrEmpty(webRequest.downloadHandler.text))
			{
				callback("<nullorempty>Error! server returned an empty response.", responseCode);
			}
			else
			{
				callback(webRequest.downloadHandler.text, responseCode);
			}
			yield break;
		}

		public static async Task SendHTTPRequestAsync(string baseUrl, SystemServices.HTTPMethod requestMethod, Action<string, HttpStatusCode?> callback, Dictionary<string, string> requestParameters, byte[] postData, string contentType, int? timeout = null, Dictionary<string, string> header = null)
		{
			SystemServices.SetPatterns();
			await Task.Delay(0);
			HttpWebRequest request;
			try
			{
				request = (HttpWebRequest)WebRequest.Create(baseUrl);
			}
			catch (Exception ex)
			{
				callback(SystemServices.regexPatterns.generalError + "+" + ex.ToString(), null);
				return;
			}
			HttpWebResponse httpResponse = null;
			try
			{
				request.Timeout = ((timeout == null) ? 100000 : timeout.Value);
				request.Method = requestMethod.methodName;
				request.Headers = new WebHeaderCollection();
				request.AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate);
				if (header != null)
				{
					foreach (KeyValuePair<string, string> keyValuePair in header)
					{
						request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
					}
				}
				if (requestParameters != null)
				{
					string queryStringFromKeyValues = SystemServices.GetQueryStringFromKeyValues(requestParameters);
					if (requestMethod.methodName == "GET")
					{
						baseUrl += queryStringFromKeyValues;
					}
					else
					{
						byte[] paramsData = Encoding.UTF8.GetBytes(queryStringFromKeyValues);
						request.ContentLength = (long)paramsData.Length;
						using (Stream stream = await request.GetRequestStreamAsync())
						{
							stream.Write(paramsData, 0, paramsData.Length);
						}
						paramsData = null;
					}
				}
				if (requestParameters == null && postData != null && requestMethod.methodName == "POST")
				{
					request.ContentLength = (long)postData.Length;
					await Task.Run(delegate()
					{
						using (Stream requestStream = request.GetRequestStream())
						{
							requestStream.Write(postData, 0, postData.Length);
						}
					});
				}
				await Task.Run(delegate()
				{
					httpResponse = (HttpWebResponse)request.GetResponse();
				});
				if (httpResponse.StatusCode != HttpStatusCode.OK)
				{
					callback(SystemServices.regexPatterns.netError + "+" + httpResponse.StatusDescription, new HttpStatusCode?(httpResponse.StatusCode));
				}
				else
				{
					callback(await new StreamReader(httpResponse.GetResponseStream()).ReadToEndAsync(), new HttpStatusCode?(httpResponse.StatusCode));
				}
				httpResponse.Dispose();
			}
			catch (Exception ex2)
			{
				HttpStatusCode? arg = (httpResponse == null) ? null : new HttpStatusCode?(httpResponse.StatusCode);
				if (ex2.InnerException is WebException || ex2.InnerException is SocketException)
				{
					WebException ex3 = ex2 as WebException;
					if (ex3.Status == WebExceptionStatus.Timeout)
					{
						callback(SystemServices.regexPatterns.generalError + "+" + ex3.ToString(), arg);
					}
					else
					{
						callback(SystemServices.regexPatterns.netError + "+" + ex3.ToString(), arg);
					}
				}
				else
				{
					callback(SystemServices.regexPatterns.generalError + "+" + ex2.ToString(), arg);
				}
			}
		}

		public static void SendHTTPRequestBlocking(string baseUrl, SystemServices.HTTPMethod requestMethod, Action<string, HttpStatusCode?> callback, Dictionary<string, string> requestParameters, byte[] postData, string contentType, int? timeout = null, Dictionary<string, string> header = null)
		{
			SystemServices.SetPatterns();
			HttpWebResponse httpWebResponse = null;
			try
			{
				if (requestParameters != null && requestMethod.methodName == "GET")
				{
					string queryStringFromKeyValues = SystemServices.GetQueryStringFromKeyValues(requestParameters);
					baseUrl = baseUrl + "?" + queryStringFromKeyValues;
				}
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(baseUrl);
				httpWebRequest.Timeout = ((timeout == null) ? 100000 : timeout.Value);
				httpWebRequest.Method = requestMethod.methodName;
				httpWebRequest.Headers = new WebHeaderCollection();
				httpWebRequest.AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate);
				httpWebRequest.ContentType = contentType;
				if (header != null)
				{
					foreach (KeyValuePair<string, string> keyValuePair in header)
					{
						httpWebRequest.Headers.Add(keyValuePair.Key, keyValuePair.Value);
					}
				}
				if (requestParameters != null && requestMethod.methodName == "POST")
				{
					string queryStringFromKeyValues2 = SystemServices.GetQueryStringFromKeyValues(requestParameters);
					byte[] bytes = Encoding.ASCII.GetBytes(queryStringFromKeyValues2);
					httpWebRequest.ContentLength = (long)bytes.Length;
					using (Stream requestStream = httpWebRequest.GetRequestStream())
					{
						requestStream.Write(bytes, 0, bytes.Length);
						goto IL_140;
					}
				}
				if (requestParameters == null && requestMethod.methodName != "GET")
				{
					httpWebRequest.ContentLength = 0L;
				}
				IL_140:
				if (requestParameters == null && postData != null && requestMethod.methodName == "POST")
				{
					httpWebRequest.ContentLength = (long)postData.Length;
					using (Stream requestStream2 = httpWebRequest.GetRequestStream())
					{
						requestStream2.Write(postData, 0, postData.Length);
					}
				}
				httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				if (httpWebResponse.StatusCode != HttpStatusCode.OK)
				{
					callback(SystemServices.regexPatterns.netError + "+" + httpWebResponse.StatusDescription, new HttpStatusCode?(httpWebResponse.StatusCode));
				}
				else
				{
					string arg = new StreamReader(httpWebResponse.GetResponseStream()).ReadToEnd();
					callback(arg, new HttpStatusCode?(httpWebResponse.StatusCode));
				}
				httpWebResponse.Dispose();
			}
			catch (Exception ex)
			{
				HttpStatusCode? arg2 = (httpWebResponse == null) ? null : new HttpStatusCode?(httpWebResponse.StatusCode);
				if (ex.InnerException is WebException || ex.InnerException is SocketException)
				{
					WebException ex2 = ex as WebException;
					if (ex2.Status == WebExceptionStatus.Timeout)
					{
						callback(SystemServices.regexPatterns.generalError + "+" + ex2.ToString(), arg2);
					}
					else
					{
						callback(SystemServices.regexPatterns.netError + "+" + ex2.ToString(), arg2);
					}
				}
				else
				{
					callback(SystemServices.regexPatterns.generalError + "+" + ex.ToString(), arg2);
				}
			}
		}

		public static async Task AsyncResourceDownload(string resourceUrl, Action<byte[], string, HttpStatusCode?> callback, int? timeout = null)
		{
			SystemServices.SetPatterns();
			await Task.Delay(0);
			HttpWebRequest request;
			try
			{
				request = (HttpWebRequest)WebRequest.Create(resourceUrl);
			}
			catch (Exception ex)
			{
				callback(null, ex.ToString(), null);
				return;
			}
			HttpWebResponse httpResponse = null;
			try
			{
				request.Timeout = ((timeout == null) ? 100000 : timeout.Value);
				await Task.Run(delegate()
				{
					httpResponse = (HttpWebResponse)request.GetResponse();
				});
				if (httpResponse.StatusCode != HttpStatusCode.OK)
				{
					callback(null, httpResponse.StatusDescription, new HttpStatusCode?(httpResponse.StatusCode));
				}
				else
				{
					Stream responseStream = httpResponse.GetResponseStream();
					byte[] arg = null;
					try
					{
						using (BinaryReader binaryReader = new BinaryReader(responseStream))
						{
							arg = binaryReader.ReadBytes((int)responseStream.Length);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogWarning(ex2);
						callback(arg, ex2.ToString(), new HttpStatusCode?(httpResponse.StatusCode));
					}
					callback(arg, "", new HttpStatusCode?(httpResponse.StatusCode));
				}
				httpResponse.Dispose();
			}
			catch (Exception ex3)
			{
				HttpStatusCode? arg2 = (httpResponse == null) ? null : new HttpStatusCode?(httpResponse.StatusCode);
				if (ex3.InnerException is WebException || ex3.InnerException is SocketException)
				{
					WebException ex4 = ex3 as WebException;
					if (ex4.Status == WebExceptionStatus.Timeout)
					{
						callback(null, ex4.ToString(), arg2);
					}
					else
					{
						callback(null, ex4.ToString(), arg2);
					}
				}
				else
				{
					callback(null, ex3.ToString(), arg2);
				}
			}
		}

		public static async Task AsyncReachabilityCheck(string testUrl, Action<bool> callback)
		{
			SystemServices.HTTPMethod requestMethod = new SystemServices.HTTPMethod(SystemServices.HTTPMethod.HTTPMethods.GET);
			await SystemServices.SendHTTPRequestAsync(testUrl, requestMethod, delegate(string response, HttpStatusCode? statusCode)
			{
				if (statusCode != null)
				{
					HttpStatusCode? httpStatusCode = statusCode;
					HttpStatusCode httpStatusCode2 = HttpStatusCode.OK;
					if (httpStatusCode.GetValueOrDefault() == httpStatusCode2 & httpStatusCode != null)
					{
						callback(true);
						return;
					}
				}
				callback(false);
			}, null, null, "application/json", null, null);
		}

		public static void BlockingReachabilityCheck(string url, Action<bool> callback)
		{
			SystemServices.HTTPMethod requestMethod = new SystemServices.HTTPMethod(SystemServices.HTTPMethod.HTTPMethods.GET);
			SystemServices.SendHTTPRequestBlocking(url, requestMethod, delegate(string response, HttpStatusCode? statusCode)
			{
				if (statusCode != null)
				{
					HttpStatusCode? httpStatusCode = statusCode;
					HttpStatusCode httpStatusCode2 = HttpStatusCode.OK;
					if (httpStatusCode.GetValueOrDefault() == httpStatusCode2 & httpStatusCode != null)
					{
						callback(true);
						return;
					}
				}
				callback(false);
			}, null, null, "application/json", null, null);
		}

		public static SystemServices.MessagePatternPair ParseResponseMessage(string message)
		{
			string patternAppended = SystemServices.regexPatterns.nothing;
			string parsedMessage;
			if (Regex.IsMatch(message, SystemServices.regexPatterns.netError, RegexOptions.Compiled))
			{
				parsedMessage = message.Replace(SystemServices.regexPatterns.netError + "+", "");
				patternAppended = SystemServices.regexPatterns.netError;
			}
			else if (Regex.IsMatch(message, SystemServices.regexPatterns.apiMistmatch, RegexOptions.Compiled))
			{
				parsedMessage = message.Replace(SystemServices.regexPatterns.apiMistmatch + "+", "");
				patternAppended = SystemServices.regexPatterns.apiMistmatch;
			}
			else if (Regex.IsMatch(message, SystemServices.regexPatterns.generalError, RegexOptions.Compiled))
			{
				parsedMessage = message.Replace(SystemServices.regexPatterns.generalError + "+", "");
				patternAppended = SystemServices.regexPatterns.generalError;
			}
			else if (Regex.IsMatch(message, SystemServices.regexPatterns.parametersMismatch, RegexOptions.Compiled))
			{
				parsedMessage = message.Replace(SystemServices.regexPatterns.parametersMismatch + "+", "");
				patternAppended = SystemServices.regexPatterns.parametersMismatch;
			}
			else if (Regex.IsMatch(message, SystemServices.regexPatterns.nullOrEmpty, RegexOptions.Compiled))
			{
				parsedMessage = message.Replace(SystemServices.regexPatterns.nullOrEmpty + "+", "");
				patternAppended = SystemServices.regexPatterns.nullOrEmpty;
			}
			else
			{
				parsedMessage = null;
				patternAppended = SystemServices.regexPatterns.nothing;
			}
			return new SystemServices.MessagePatternPair(patternAppended, parsedMessage);
		}

		public static bool IsSuccessStatusCode(long statusCode)
		{
			return (int)statusCode >= 200 && (int)statusCode <= 299;
		}

		public static string GetQueryStringFromKeyValues(Dictionary<string, string> parameters)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, string> keyValuePair in parameters)
			{
				list.Add(keyValuePair.Key + "=" + Uri.EscapeDataString(keyValuePair.Value));
			}
			return string.Join("&", list);
		}

		public static async Task RunDelayedCommand(int secs, Action command)
		{
			await Task.Delay(secs * 1000);
			command();
		}

		public static byte[] ReadAllBytes(Stream source)
		{
			long position = source.Position;
			source.Position = 0L;
			byte[] result;
			try
			{
				byte[] array = new byte[4096];
				int num = 0;
				int num2;
				while ((num2 = source.Read(array, num, array.Length - num)) > 0)
				{
					num += num2;
					if (num == array.Length)
					{
						int num3 = source.ReadByte();
						if (num3 != -1)
						{
							byte[] array2 = new byte[array.Length * 2];
							Buffer.BlockCopy(array, 0, array2, 0, array.Length);
							Buffer.SetByte(array2, num, (byte)num3);
							array = array2;
							num++;
						}
					}
				}
				byte[] array3 = array;
				if (array.Length != num)
				{
					array3 = new byte[num];
					Buffer.BlockCopy(array, 0, array3, 0, num);
				}
				result = array3;
			}
			finally
			{
				source.Position = position;
			}
			return result;
		}

		public static async Task WriteTextureAsync(Texture2D texture, SystemServices.ImageFormat format, string fileName, string path, Action<string> callback)
		{
			try
			{
				byte[] data = null;
				switch (format)
				{
				case SystemServices.ImageFormat.PNG:
					data = texture.EncodeToPNG();
					if (!fileName.ToLower().Contains(".png"))
					{
						fileName += ".png";
					}
					break;
				case SystemServices.ImageFormat.JPG:
					data = texture.EncodeToJPG();
					if (!fileName.ToLower().Contains(".jpg"))
					{
						fileName += ".jpg";
					}
					break;
				case SystemServices.ImageFormat.EXR:
					data = texture.EncodeToEXR();
					if (!fileName.ToLower().Contains(".exr"))
					{
						fileName += ".exr";
					}
					break;
				}
				if (data == null)
				{
					Debug.Log("Failed encoding");
				}
				if (path.EndsWith("/") || path.EndsWith("\\"))
				{
					path += fileName;
				}
				else
				{
					path = path + "/" + fileName;
				}
				using (FileStream fileStream = File.Create(path))
				{
					await fileStream.WriteAsync(data, 0, data.Length);
				}
				FileStream fileStream = null;
				callback("");
				Debug.Log(data.Length / 1024 + "Kb was saved as: " + path);
				data = null;
			}
			catch (Exception ex)
			{
				callback(ex.ToString());
			}
		}

		public static async Task WriteBytesAsync(byte[] data, string fullPath, Action<string> callback)
		{
			try
			{
				using (FileStream fileStream = File.Create(fullPath))
				{
					await fileStream.WriteAsync(data, 0, data.Length);
				}
				FileStream fileStream = null;
				callback("");
				Debug.Log(data.Length / 1024 + "Kb was saved as: " + fullPath);
			}
			catch (Exception ex)
			{
				callback(ex.ToString());
			}
		}

		public static SystemServices.RegexPatterns regexPatterns;

		[Serializable]
		public struct RegexPatterns
		{
			public string netError;

			public string nullOrEmpty;

			public string generalError;

			public string apiMistmatch;

			public string parametersMismatch;

			public string nothing;
		}

		public struct MessagePatternPair
		{
			public string patternAppended { get; private set; }

			public string parsedMessage { get; private set; }

			public MessagePatternPair(string patternAppended, string parsedMessage)
			{
				this.patternAppended = patternAppended;
				this.parsedMessage = parsedMessage;
			}
		}

		public class HTTPMethod
		{
			public HTTPMethod(SystemServices.HTTPMethod.HTTPMethods method)
			{
				this.methodName = Enum.GetName(typeof(SystemServices.HTTPMethod.HTTPMethods), method);
			}

			public readonly string methodName;

			public enum HTTPMethods
			{
				POST,
				GET
			}
		}

		public enum ImageFormat
		{
			PNG,
			JPG,
			EXR
		}
	}
}
