﻿using System;
using System.Runtime.InteropServices;

namespace AVDump2Lib.BlockConsumers.Ogg.BitStreams {
    public class OGMTextOGGBitStream : SubtitleOGGBitStream, IOGMStream, IVorbisComment {
		public override string CodecName { get { return "OGMText"; } }
		public override string CodecVersion { get; protected set; }
		public string ActualCodecName { get; private set; }

		public OGMTextOGGBitStream(byte[] header, int offset)
			: base(false) {
			var codecInfo = GetStruct<OGMTextHeader>(header, offset + 1, 0x38);
			ActualCodecName = new string(codecInfo.SubType);
		}

		[StructLayout(LayoutKind.Sequential, Size = 52)]
		public struct OGMTextHeader {
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public Char[] StreamType;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public Char[] SubType;
			public Int32 Size;
			public Int64 TimeUnit;
			public Int64 SamplesPerUnit;
			public Int32 DefaultLength;
			public Int32 BufferSize;
			public Int16 BitsPerSample;
			public Int64 Unused;
		}

		internal override void ProcessPage(Page page) {
			base.ProcessPage(page);
			commentParser.ParsePage(page);
		}

		private VorbisCommentParser commentParser = new VorbisCommentParser();
		public Comments Comments {
			get {
				try {
					return commentParser.RetrieveComments();
				} catch(Exception) {
					return null; //TODO: Log warning
				}
			}
		}
	}
}
