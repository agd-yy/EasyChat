﻿using EasyChat.Utilities;
using System.Net.NetworkInformation;
using System.Net;

namespace EasyChat.Models;

/// <summary>
///     MQTT 静态常量类
/// </summary>
public static class MqttContent
{
    // 1.主题订阅相关
    // 上线
    public const string ONLINE = "online/";
    // 分组
    public const string GROUP = "group/";
    // 消息
    public const string MESSAGE = "message/";
    // 文件
    public const string FILE = "file/";

    // 2.其他
    public const int SERVER_PORT = 1883;
    public const int SOCKET_PORT = 9100;
    public const string SERVER_USER = "Doctor";
    public const string SERVER_PW = "Amiya1223";
    public const string OFFLINE_STRING = "(已离线)";
    public const string FILE_STRING = "[文件]";

    public static string IPADDRESS = string.Empty;
    public static string PASSWORD = string.Empty;
    public static string USER_NAME = string.Empty;

    // 生成随机头像用
    public static string GetRandomImg()
    {
        Random _random = new Random();
        return "/Resources/Images/p" + _random.Next(1, 13) + ".jpg";
    }

    public static string GetLocalIp(string serverIP)
    {
        if (string.IsNullOrEmpty(serverIP)) return "";
        var ipList = NetworkUtilities.GetIps();
        return ipList.FirstOrDefault(x => x.StartsWith(serverIP.Split('.')[0])) ?? "";
    }

    public static int GetLocalOkPort(int port, IPEndPoint[]? ipEndPoints)
    {
        if (ipEndPoints == null)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            ipEndPoints = ipProperties.GetActiveTcpListeners();
        }
        foreach (IPEndPoint endPoint in ipEndPoints)
        {
            if (endPoint.Port == port)
            {
                return GetLocalOkPort(++port, ipEndPoints);
            }
        }
        return port;
    }

    /// <summary>
    /// MsgModel -> ChatMessage
    /// </summary>
    /// <param name="msgModel"></param>
    /// <param name="myUid"></param>
    /// <returns></returns>
    public static ChatMessage ToChatMessage(MsgModel msgModel, string myUid)
    {
        return new ChatMessage
        {
            NickName = msgModel.userModel.nickName,
            Image = string.IsNullOrEmpty(msgModel.userModel.image) ? GetRandomImg() : msgModel.userModel.image,
            Message = msgModel.message,
            Time = msgModel.sendTime.ToString(),
            IsMyMessage = msgModel.userModel.uid == myUid,
            IsFile = msgModel.isImageOrFile,
            FilePath = msgModel.clientFilePath,
            FileSize = msgModel.fileSize,
            FileName = msgModel.fileName,
            UserModel = msgModel.userModel
        };
    }

    /// <summary>
    /// ChatModel -> UserModel
    /// </summary>
    /// <param name="chatModel"></param>
    /// <returns></returns>
    public static UserModel ToUserModel(ChatModel chatModel)
    {
        return new UserModel()
        {
            uid = chatModel.Uid,
            nickName = chatModel.NickName,
            image = chatModel.Image,
            ipAddress = chatModel.IpAddress,
            port = chatModel.Port
        };
    }

    /// <summary>
    /// UserModel -> ChatModel
    /// </summary>
    /// <param name="userModel"></param>
    /// <returns></returns>
    public static ChatModel ToChatModel(UserModel userModel)
    {
        return new ChatModel()
        {
            GroupName = userModel.nickName,
            Uid = userModel.uid,
            Image = userModel.image,
            NickName = userModel.isOnline ? userModel.nickName : userModel.nickName + OFFLINE_STRING,
            IsOnline = userModel.isOnline,
            IsGroup = userModel.isGroup,
            IpAddress = userModel.ipAddress,
            Port = userModel.port
        };
    }

    /// <summary>
    /// 文件大小转字符串
    /// </summary>
    /// <param name="fileSize"></param>
    /// <returns></returns>
    public static string FileSizeToString(long fileSize)
    {
        if (fileSize < 1024)
        { 
            return $"{fileSize}B";
        }
        else if (fileSize < 1048576)
        {
            return $"{(fileSize / 1024f):f2}KB";
        }
        else if (fileSize < 1073741824)
        { 
            return $"{(fileSize / 1048576f):f2}MB"; 
        }
        else
        { 
            return $"{(fileSize / 1073741824f):f2}GB";
        }
    }
}