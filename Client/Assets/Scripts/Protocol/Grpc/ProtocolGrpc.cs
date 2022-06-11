﻿// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: sources/Protocol.proto
// </auto-generated>
// Original file comments:
// 型:https://qiita.com/yukina-ge/items/98fe190cef2024d45fbd
// 書き方:https://qiita.com/Captain_Blue/items/b7a1f4a42f48559fac0c
//
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace Server.gRPC {
  public static partial class Unary
  {
    static readonly string __ServiceName = "gRPC.Unary";

    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    static readonly grpc::Marshaller<global::Server.gRPC.C2S_Ping_Request> __Marshaller_gRPC_C2S_Ping_Request = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Server.gRPC.C2S_Ping_Request.Parser));
    static readonly grpc::Marshaller<global::Server.gRPC.S2C_Ping_Response> __Marshaller_gRPC_S2C_Ping_Response = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Server.gRPC.S2C_Ping_Response.Parser));

    static readonly grpc::Method<global::Server.gRPC.C2S_Ping_Request, global::Server.gRPC.S2C_Ping_Response> __Method_Ping = new grpc::Method<global::Server.gRPC.C2S_Ping_Request, global::Server.gRPC.S2C_Ping_Response>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Ping",
        __Marshaller_gRPC_C2S_Ping_Request,
        __Marshaller_gRPC_S2C_Ping_Response);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Server.gRPC.ProtocolReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of Unary</summary>
    [grpc::BindServiceMethod(typeof(Unary), "BindService")]
    public abstract partial class UnaryBase
    {
      /// <summary>
      /// 単項Ping
      /// </summary>
      /// <param name="request">The request received from the client.</param>
      /// <param name="context">The context of the server-side call handler being invoked.</param>
      /// <returns>The response to send back to the client (wrapped by a task).</returns>
      public virtual global::System.Threading.Tasks.Task<global::Server.gRPC.S2C_Ping_Response> Ping(global::Server.gRPC.C2S_Ping_Request request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for Unary</summary>
    public partial class UnaryClient : grpc::ClientBase<UnaryClient>
    {
      /// <summary>Creates a new client for Unary</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public UnaryClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for Unary that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public UnaryClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected UnaryClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected UnaryClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      /// <summary>
      /// 単項Ping
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The response received from the server.</returns>
      public virtual global::Server.gRPC.S2C_Ping_Response Ping(global::Server.gRPC.C2S_Ping_Request request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return Ping(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// 単項Ping
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The response received from the server.</returns>
      public virtual global::Server.gRPC.S2C_Ping_Response Ping(global::Server.gRPC.C2S_Ping_Request request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Ping, null, options, request);
      }
      /// <summary>
      /// 単項Ping
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncUnaryCall<global::Server.gRPC.S2C_Ping_Response> PingAsync(global::Server.gRPC.C2S_Ping_Request request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return PingAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// 単項Ping
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncUnaryCall<global::Server.gRPC.S2C_Ping_Response> PingAsync(global::Server.gRPC.C2S_Ping_Request request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Ping, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override UnaryClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new UnaryClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(UnaryBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_Ping, serviceImpl.Ping).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static void BindService(grpc::ServiceBinderBase serviceBinder, UnaryBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_Ping, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::Server.gRPC.C2S_Ping_Request, global::Server.gRPC.S2C_Ping_Response>(serviceImpl.Ping));
    }

  }
  public static partial class ServerStreaming
  {
    static readonly string __ServiceName = "gRPC.ServerStreaming";


    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Server.gRPC.ProtocolReflection.Descriptor.Services[1]; }
    }

    /// <summary>Base class for server-side implementations of ServerStreaming</summary>
    [grpc::BindServiceMethod(typeof(ServerStreaming), "BindService")]
    public abstract partial class ServerStreamingBase
    {
    }

    /// <summary>Client for ServerStreaming</summary>
    public partial class ServerStreamingClient : grpc::ClientBase<ServerStreamingClient>
    {
      /// <summary>Creates a new client for ServerStreaming</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public ServerStreamingClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for ServerStreaming that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public ServerStreamingClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected ServerStreamingClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected ServerStreamingClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override ServerStreamingClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new ServerStreamingClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(ServerStreamingBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder().Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static void BindService(grpc::ServiceBinderBase serviceBinder, ServerStreamingBase serviceImpl)
    {
    }

  }
  /// <summary>
  ///*****************************************************************
  /// CLストリーミング : SV1 対 CL多
  ///*****************************************************************
  /// </summary>
  public static partial class ClientStreaming
  {
    static readonly string __ServiceName = "gRPC.ClientStreaming";


    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Server.gRPC.ProtocolReflection.Descriptor.Services[2]; }
    }

    /// <summary>Base class for server-side implementations of ClientStreaming</summary>
    [grpc::BindServiceMethod(typeof(ClientStreaming), "BindService")]
    public abstract partial class ClientStreamingBase
    {
    }

    /// <summary>Client for ClientStreaming</summary>
    public partial class ClientStreamingClient : grpc::ClientBase<ClientStreamingClient>
    {
      /// <summary>Creates a new client for ClientStreaming</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public ClientStreamingClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for ClientStreaming that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public ClientStreamingClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected ClientStreamingClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected ClientStreamingClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override ClientStreamingClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new ClientStreamingClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(ClientStreamingBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder().Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static void BindService(grpc::ServiceBinderBase serviceBinder, ClientStreamingBase serviceImpl)
    {
    }

  }
  public static partial class BidirectionalStreaming
  {
    static readonly string __ServiceName = "gRPC.BidirectionalStreaming";

    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    static readonly grpc::Marshaller<global::Server.gRPC.DuplexChatSend> __Marshaller_gRPC_DuplexChatSend = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Server.gRPC.DuplexChatSend.Parser));
    static readonly grpc::Marshaller<global::Server.gRPC.DuplexChatReceive> __Marshaller_gRPC_DuplexChatReceive = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Server.gRPC.DuplexChatReceive.Parser));

    static readonly grpc::Method<global::Server.gRPC.DuplexChatSend, global::Server.gRPC.DuplexChatReceive> __Method_DuplexChat = new grpc::Method<global::Server.gRPC.DuplexChatSend, global::Server.gRPC.DuplexChatReceive>(
        grpc::MethodType.DuplexStreaming,
        __ServiceName,
        "DuplexChat",
        __Marshaller_gRPC_DuplexChatSend,
        __Marshaller_gRPC_DuplexChatReceive);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Server.gRPC.ProtocolReflection.Descriptor.Services[3]; }
    }

    /// <summary>Base class for server-side implementations of BidirectionalStreaming</summary>
    [grpc::BindServiceMethod(typeof(BidirectionalStreaming), "BindService")]
    public abstract partial class BidirectionalStreamingBase
    {
      /// <summary>
      /// 双方向チャット
      /// </summary>
      /// <param name="requestStream">Used for reading requests from the client.</param>
      /// <param name="responseStream">Used for sending responses back to the client.</param>
      /// <param name="context">The context of the server-side call handler being invoked.</param>
      /// <returns>A task indicating completion of the handler.</returns>
      public virtual global::System.Threading.Tasks.Task DuplexChat(grpc::IAsyncStreamReader<global::Server.gRPC.DuplexChatSend> requestStream, grpc::IServerStreamWriter<global::Server.gRPC.DuplexChatReceive> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for BidirectionalStreaming</summary>
    public partial class BidirectionalStreamingClient : grpc::ClientBase<BidirectionalStreamingClient>
    {
      /// <summary>Creates a new client for BidirectionalStreaming</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public BidirectionalStreamingClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for BidirectionalStreaming that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public BidirectionalStreamingClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected BidirectionalStreamingClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected BidirectionalStreamingClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      /// <summary>
      /// 双方向チャット
      /// </summary>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncDuplexStreamingCall<global::Server.gRPC.DuplexChatSend, global::Server.gRPC.DuplexChatReceive> DuplexChat(grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return DuplexChat(new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// 双方向チャット
      /// </summary>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncDuplexStreamingCall<global::Server.gRPC.DuplexChatSend, global::Server.gRPC.DuplexChatReceive> DuplexChat(grpc::CallOptions options)
      {
        return CallInvoker.AsyncDuplexStreamingCall(__Method_DuplexChat, null, options);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override BidirectionalStreamingClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new BidirectionalStreamingClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(BidirectionalStreamingBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_DuplexChat, serviceImpl.DuplexChat).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static void BindService(grpc::ServiceBinderBase serviceBinder, BidirectionalStreamingBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_DuplexChat, serviceImpl == null ? null : new grpc::DuplexStreamingServerMethod<global::Server.gRPC.DuplexChatSend, global::Server.gRPC.DuplexChatReceive>(serviceImpl.DuplexChat));
    }

  }
}
#endregion
