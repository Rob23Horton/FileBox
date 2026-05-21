using FileBox.Client.Pages;
using FileBox.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Net.Http.Json;

namespace FileBox.Client.Services;

public class TagService : IDisposable
{
	public TagService(NavigationManager NavigationManager, HttpClient HttpClient)
	{
		_navigationManager = NavigationManager;
		_httpClient = HttpClient;

		_hub = new HubConnectionBuilder()
			.WithUrl($"{_navigationManager.BaseUri}hubs/notifications")
			.WithAutomaticReconnect()
			.Build();

		_hub.On("TagsChanged", async () =>
		{
			await GetTagsFromAPI();
		});
	}

	#region Private Properties

	private NavigationManager _navigationManager;
	private HubConnection _hub;
	private HttpClient _httpClient;

	private List<Tag> _tags = new List<Tag>();

	#endregion

	#region Events

	public event Action? TagsUpdated;

	#endregion

	public List<Tag> GetRootTags()
	{
		try
		{
			GetTagsFromAPI();

			return _tags.Where(t => t.ParentTagCode is null).ToList();
		}
		catch
		{

		}
		return [];
	}

	public List<Tag> GetChildTags(int TagId)
	{
		try
		{
			if (_tags.Where(t => t.ParentTagCode == TagId).ToList() is { Count: >0 } and var childTags)
			{
				return childTags;
			}

			GetTagsFromAPI();
		}
		catch
		{

		}
		return [];
	}

	public async Task GetTagsFromAPI()
	{
		try
		{
			//Call API
			List<Tag> newTags = await _httpClient.GetFromJsonAsync<List<Tag>>("http://localhost:5072/api/tag") ?? [];

			if (newTags.Count() == _tags.Count())
				return;

			_tags = newTags;

			//Tell components to update
			if (TagsUpdated is not null)
				TagsUpdated?.Invoke();
		}
		catch
		{

		}
	}


	public void Dispose()
	{
		_hub.DisposeAsync();
	}
}
