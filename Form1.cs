using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using YTChat;
using static YTChat.Form1;

namespace YTChat
{
	public partial class Form1 : Form
	{

		public class Tokens
		{
			public int Index { get; set; }
			public string Name { get; set; }
			public string AccessToken { get; set; }
			public string ResponsePath { get; set; }
		}

		static string jsonFilePath = "token/access_token.json";
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			LoadJSON();
		}
		private void LoadJSON()
		{
			Accounts.Items.Clear();
			List<Tokens> tokensList;
			string jsonContent = File.ReadAllText(jsonFilePath);
			tokensList = JsonConvert.DeserializeObject<List<Tokens>>(jsonContent);
			foreach (var name in tokensList)
			{
				Accounts.Items.Add(name.Name);
			}
		}

		public void GetYouTubeChannelToken(int index)
		{
			// ���� � ����� � ����������� �������� �������
			string credentialsPath = "token.json";
			string responsesPath = "token/"+index;
			// ������ ���������� �� ������ � YouTube Data API
			string[] scopes = { "https://www.googleapis.com/auth/youtube.force-ssl", "https://www.googleapis.com/auth/youtube" };
			// �������� ���������� ������� ������ �� �����
			UserCredential credential;
			using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
			{
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					scopes,
					"user",
					CancellationToken.None,
					new FileDataStore(responsesPath, true)).Result;
				stream.Dispose();
			}
			var youtubeService = new YouTubeService(new BaseClientService.Initializer
			{
				HttpClientInitializer = credential
			});

			var channelsListRequest = youtubeService.Channels.List("snippet");
			channelsListRequest.Mine = true;
			var channelsListResponse = channelsListRequest.Execute();
			string channelName = channelsListResponse.Items[0].Snippet.Title;

			string accessToken = credential.Token.AccessToken;
			string path = responsesPath;
			SaveJSON(channelName, accessToken, path);
			LoadJSON();
		}

		public static string ChatID(string apiKey, string videoId)
		{
			// ������������� YouTubeService � �������������� ����� API
			YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
			{
				ApiKey = apiKey
			});

			// ������� IDVideo, ��� �������� ������ �������� LIVE_CHAT_ID


			// ���������� ������� �� ��������� ���������� � �����
			var videoRequest = youtubeService.Videos.List("liveStreamingDetails");
			videoRequest.Id = videoId;
			var videoResponse = videoRequest.Execute();

			// ��������� LIVE_CHAT_ID �� ������
			var liveChatId = videoResponse.Items[0].LiveStreamingDetails.ActiveLiveChatId;

			return liveChatId;
		}
		
		static void SaveJSON(string nameToken, string accessToken, string path)
		{
			// ��������� ������������ JSON-���� � ������������� ��� � ������ �������� Tokens
			List<Tokens> tokensList;

			// ���������, ���������� �� ����
			if (File.Exists(jsonFilePath))
			{
				string jsonContent = File.ReadAllText(jsonFilePath);
				tokensList = JsonConvert.DeserializeObject<List<Tokens>>(jsonContent);

				if (tokensList == null)
					tokensList = new List<Tokens>();
			}
			else
			{
				// ���� ���� �� ����������, ������� ����� ������
				tokensList = new List<Tokens>();
			}
			tokensList.Add(new Tokens
			{
				Index = tokensList.Count, // ���������� ����� ������ (������������, ��� ������� ���������� � 0)
				Name = nameToken, // �������� �� ������ ���
				AccessToken = accessToken, // �������� �� ������ access token
				ResponsePath = path// �������� �� ������ refresh token
			});
			// ����������� ������ � JSON-������
			string jsonTokens = JsonConvert.SerializeObject(tokensList, Formatting.Indented);

			// ���������� JSON-������ � ����
			File.WriteAllText(jsonFilePath, jsonTokens);
		}

		public void SendMessage(string token)
		{
			string apiKey = "AIzaSyBKCPadLlScZMcY41RVWk_DIaR7s3kM-sk"; // ApiKey
			string videoId = videoIdText.Text; // ����� �����
			try
			{
				string liveChatId = ChatID(apiKey, videoId); // �������� �� ID ���� ������ ����������
				string messageText = textBox1.Text; // �������� �� ����� ������ ���������

				var httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				var content = new StringContent("{\"snippet\": {\"liveChatId\": \"" + liveChatId + "\", \"type\": \"textMessageEvent\", \"textMessageDetails\": {\"messageText\": \"" + messageText + "\"}}}", Encoding.UTF8, "application/json");

				// ������� �������� part=snippet
				var requestUrl = "https://www.googleapis.com/youtube/v3/liveChat/messages?part=snippet";

				var response = httpClient.PostAsync(requestUrl, content).Result;

				if (response.IsSuccessStatusCode)
				{

				}
				else
				{
					MessageBox.Show("������ ��� �������� ���������. ��� ������: " + response.StatusCode);
					// �������� ���������� ������ ��� �������������� �����������
					var responseContent = response.Content.ReadAsStringAsync().Result;
					MessageBox.Show("����� �������: " + responseContent);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("An error occurred: " + ex.Message);
			}
		}
		public UserCredential GetUpdatedUserCredential(string[] tokensPath)
		{
			string credentialsPath = "token.json";
			UserCredential userCredential;
			using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
			{
				// ��������� ������� ������ OAuth 2.0 �� JSON-�����
				var clientSecrets = GoogleClientSecrets.Load(stream).Secrets;

				// �������� ������ TokenResponse �� ����� � ������� ������������ (TokenResponse-user)
				var tokenResponseJson = File.ReadAllText(tokensPath[0]);
				var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(tokenResponseJson);

				// �������� ������ UserCredential �� ������ ������� ������ � TokenResponse
				userCredential = new UserCredential(new GoogleAuthorizationCodeFlow(
			   new GoogleAuthorizationCodeFlow.Initializer
			   {
				   DataStore = new FileDataStore("YouTubeAPI", false),
				   ClientSecrets = clientSecrets
			   }),
			   "user",
			   tokenResponse);


				// ���������� �������� Access Token � �������������� Refresh Token
				userCredential.RefreshTokenAsync(CancellationToken.None);





				// ���������� ����������� ������ ������������ � ���� (���� ����������)
				// ��� �������������� ���, ���� �� ������ �������� ������ � ����� TokenResponse-user
				//var updatedTokenResponseJson = JsonConvert.SerializeObject(userCredential.Token);
				//File.WriteAllText(tokensPath[0], updatedTokenResponseJson
			}
			return userCredential;

		}
		
		private void button3_Click(object sender, EventArgs e)
		{
			GetYouTubeChannelToken(Accounts.Items.Count);
		}

		private async void button2_Click(object sender, EventArgs e)
		{
			await GetUpdatedAccessToken(Accounts.SelectedIndex);

			var token = GetUpdatedUserCredential(Directory.GetFiles("token/" + Accounts.SelectedIndex + "/", "*.TokenResponse-user")).Token.AccessToken;
			if (videoIdText.Text.Length > 0)
			{
				SendMessage(token);
			}
		}
		public static async Task<UserCredential> GetUserCredential(int index)
		{
			
			using (var stream = new FileStream("token.json", FileMode.Open, FileAccess.Read))
			{
				var clientSecrets = GoogleClientSecrets.Load(stream).Secrets;
				string[] files = Directory.GetFiles("token/" + index + "/", "*.TokenResponse-user");
				string tokensPath = files[0];
				
				var tokenResponseJson = File.ReadAllText(tokensPath);
				var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(tokenResponseJson);

				var userCredential = new UserCredential(new GoogleAuthorizationCodeFlow(
					new GoogleAuthorizationCodeFlow.Initializer
					{
						DataStore = new FileDataStore("YouTubeAPI", false),
						ClientSecrets = clientSecrets
					}),
					"user",
					tokenResponse);

				// ���������, ����� �� Access Token
				if (userCredential.Token.IsExpired(SystemClock.Default))
				{
					// ���������� �������� Access Token � �������������� Refresh Token
					bool success = await userCredential.RefreshTokenAsync(CancellationToken.None);
					if (!success)
					{
						throw new Exception("Failed to refresh the token.");
					}

					var updatedTokenResponseJson = Newtonsoft.Json.JsonConvert.SerializeObject(userCredential.Token);
					File.WriteAllText(tokensPath, updatedTokenResponseJson);
				}

				return userCredential;
			}
		}

		public static async Task<string> GetUpdatedAccessToken(int index)
		{
			UserCredential credential = await GetUserCredential(index);
			return credential.Token.AccessToken;
		}	

		private void button1_Click(object sender, EventArgs e)
		{

		}
	}
}
