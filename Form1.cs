using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.AccessControl;
using System.Text;
using YTChat;

namespace YTChat
{
	public partial class Form1 : Form
	{

		public class Tokens
		{
			public int Index { get; set; }
			public string Name { get; set; }
			public string AccessToken { get; set; }
		}

		static string jsonFilePath = "token/access_token.json";
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			List<Tokens> tokensList;
			string jsonContent = File.ReadAllText(jsonFilePath);
			tokensList = JsonConvert.DeserializeObject<List<Tokens>>(jsonContent);
			foreach (var name in tokensList)
			{
				Accounts.Items.Add(name.Name);
			}
		}

		public void GetYouTubeChannelToken()
		{
			// ���� � ����� � ����������� �������� �������
			string credentialsPath = "YT.json";
			// ������ ���������� �� ������ � YouTube Data API
			string[] scopes = { "https://www.googleapis.com/auth/youtube.force-ssl", "https://www.googleapis.com/auth/youtube" };
			// ���������� NullDataStore ������ FileDataStore
			IDataStore nullDataStore = new NullDataStore();
			// �������� ���������� ������� ������ �� �����
			UserCredential credential;
			using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
			{
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					scopes,
					"user",
					CancellationToken.None,
					nullDataStore).Result;
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
			SaveJSON(channelName, accessToken);
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

		static void SaveJSON(string nameToken, string accessToken)
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
				AccessToken = accessToken // �������� �� ������ access token
			});
			// ����������� ������ � JSON-������
			string jsonTokens = JsonConvert.SerializeObject(tokensList, Formatting.Indented);

			// ���������� JSON-������ � ����
			File.WriteAllText(jsonFilePath, jsonTokens);
		}

		static List<Tokens> LoadTokensListFromFile(string filePath)
		{
			// ���� ���� �� ����������, ������ ������ ������
			if (!File.Exists(filePath))
				return new List<Tokens>();

			// ��������� JSON-���� � ������������� ��� � ������ �������� Tokens
			string jsonContent = File.ReadAllText(filePath);
			List<Tokens> tokensList = JsonConvert.DeserializeObject<List<Tokens>>(jsonContent);

			return tokensList;
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
		private void button3_Click(object sender, EventArgs e)
		{
			GetYouTubeChannelToken();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			List<Tokens> tokensList;
			string jsonContent = File.ReadAllText(jsonFilePath);
			tokensList = JsonConvert.DeserializeObject<List<Tokens>>(jsonContent);
			string accessTokenAccount = tokensList[Accounts.SelectedIndex].AccessToken;

			SendMessage(accessTokenAccount);
		}

		private void button1_Click(object sender, EventArgs e)
		{

		}
	}
}
