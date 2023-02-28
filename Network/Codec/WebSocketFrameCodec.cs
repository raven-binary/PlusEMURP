﻿using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Transport.Channels;

namespace Plus.Network.Codec
{
    internal class WebSocketFrameCodec : MessageToMessageCodec<WebSocketFrame, IByteBuffer>
    {
        protected override void Decode(IChannelHandlerContext ctx, WebSocketFrame msg, List<object> output)
        {
            output.Add(msg.Content.Retain());
        }

        protected override void Encode(IChannelHandlerContext ctx, IByteBuffer msg, List<object> output)
        {
            output.Add(new BinaryWebSocketFrame(msg).Retain());
        }
    }
}