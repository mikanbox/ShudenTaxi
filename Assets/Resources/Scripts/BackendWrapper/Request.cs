using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RequestStatus
{
	BeforeSend,
	Sending,
	Success,
	Failure,
}

public abstract class Request
{
	public Request()
	{
		status = RequestStatus.BeforeSend;
	}

	private RequestStatus _status;
	public RequestStatus status { get; protected set; }
}

public class MakeIDRequest : Request
{
	public MakeIDRequest()
	{
		RequestSender.Instance.SubmitMakeIDRequest += OnSendRequest;
	}

	private void OnSendRequest()
	{
		status = RequestStatus.Sending;

		status = RequestStatus.Success;
	}
}

public class MatchingRequest : Request
{

	public MatchingRequest()
	{

		RequestSender.Instance.SubmitMatchingRequest += OnSendRequest;
	}

	public class RequestData
	{
		public int userid;
		public float here_lat;
		public float here_lng;
		public float obj_lat;
		public float obj_lng;
		public string comment;
	}

	private void OnSendRequest(RequestData data)
	{

	}
}

public class SettingConfirmRequest : Request
{
	public SettingConfirmRequest()
	{
		RequestSender.Instance.SubmitSettingConfirmRequest += OnSendRequest;
	}

	public class RequestData
	{
		public float obj_lat;
		public float obj_lng;
	}

	public void OnSendRequest(SettingConfirmRequest.RequestData data)
	{
		
	}
}

public class ChangeToSettingUIRequest : Request
{ 
	public ChangeToSettingUIRequest()
	{
		
	}

	public void OnSendRequest()
	{ 
		
	}
}

public class ChangeToCommentUIRequest : Request
{ 
	public ChangeToCommentUIRequest()
	{

	}

	public void OnSendRequest() 
	{ 
		
	}
}