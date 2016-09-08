using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;

namespace GoldEngine
{
    internal sealed class SimpleDB
    {
        // Fields
        private const short kRecordContentMulti = 0x4d;

        // Nested Types
        public class Entry
        {
            // Fields
            public SimpleDB.EntryType Type;
            public object Value;

            // Methods
            public Entry()
            {
                this.Type = SimpleDB.EntryType.Empty;
                this.Value = "";
            }

            public Entry(SimpleDB.EntryType Type, object Value)
            {
                this.Type = Type;
                this.Value = RuntimeHelpers.GetObjectValue(Value);
            }
        }

        public class EntryList : ArrayList
        {
            // Methods
            public int Add(SimpleDB.Entry Value)
            {
                return base.Add(Value);
            }

            // Properties
            public new SimpleDB.Entry this[int Index]
            {
                get
                {
                    return (SimpleDB.Entry)base[Index];
                }
                set
                {
                    base[Index] = value;
                }
            }
        }

        public enum EntryType : byte
        {
            Boolean = 0x42,
            Byte = 0x62,
            Empty = 0x45,
            Error = 0,
            String = 0x53,
            UInt16 = 0x49
        }

        public class IOException : Exception
        {
            // Methods
            public IOException(string Message) : base(Message)
            {
            }

            public IOException(SimpleDB.EntryType Type, BinaryReader Reader) : base("Type mismatch in file. Read '" + Conversions.ToString(Strings.ChrW((int)Type)) + "' at " + Conversions.ToString(Reader.BaseStream.Position))
            {
            }

            public IOException(string Message, Exception Inner) : base(Message, Inner)
            {
            }
        }

        public class Reader
        {
            // Fields
            private const byte kRecordContentMulti = 0x4d;
            private int m_EntriesRead;
            private int m_EntryCount;
            private string m_FileHeader;
            private BinaryReader m_Reader;

            // Methods
            public void Close()
            {
                if (this.m_Reader != null)
                {
                    this.m_Reader.Close();
                    this.m_Reader = null;
                }
            }

            public bool EndOfFile()
            {
                return (this.m_Reader.BaseStream.Position == this.m_Reader.BaseStream.Length);
            }

            public short EntryCount()
            {
                return (short)this.m_EntryCount;
            }

            ~Reader()
            {
                this.Close();
            }

            public bool GetNextRecord()
            {
                while (this.m_EntriesRead < this.m_EntryCount)
                {
                    this.RetrieveEntry();
                }
                if (this.m_Reader.ReadByte() == 0x4d)
                {
                    this.m_EntryCount = this.RawReadUInt16();
                    this.m_EntriesRead = 0;
                    return true;
                }
                return false;
            }

            public string Header()
            {
                return this.m_FileHeader;
            }

            public void Open(BinaryReader Reader)
            {
                this.m_Reader = Reader;
                this.m_EntryCount = 0;
                this.m_EntriesRead = 0;
                this.m_FileHeader = this.RawReadCString();
            }

            public void Open(string Path)
            {
                try
                {
                    this.Open(new BinaryReader(File.Open(Path, FileMode.Open, FileAccess.Read)));
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    throw exception;
                }
            }

            private string RawReadCString()
            {
                string str2 = "";
                bool flag = false;
                while (!flag)
                {
                    ushort charCode = this.RawReadUInt16();
                    if (charCode == 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        str2 = str2 + Conversions.ToString(Strings.ChrW(charCode));
                    }
                }
                return str2;
            }

            private ushort RawReadUInt16()
            {
                int num = this.m_Reader.ReadByte();
                return (ushort)((this.m_Reader.ReadByte() << 8) + num);
            }

            public bool RecordComplete()
            {
                return (this.m_EntriesRead >= this.m_EntryCount);
            }

            public bool RetrieveBoolean()
            {
                SimpleDB.Entry entry = this.RetrieveEntry();
                if (entry.Type != SimpleDB.EntryType.Boolean)
                {
                    throw new SimpleDB.IOException(entry.Type, this.m_Reader);
                }
                return Conversions.ToBoolean(entry.Value);
            }

            public byte RetrieveByte()
            {
                SimpleDB.Entry entry = this.RetrieveEntry();
                if (entry.Type != SimpleDB.EntryType.Byte)
                {
                    throw new SimpleDB.IOException(entry.Type, this.m_Reader);
                }
                return Conversions.ToByte(entry.Value);
            }

            public SimpleDB.Entry RetrieveEntry()
            {
                SimpleDB.Entry entry = new SimpleDB.Entry();
                if (this.RecordComplete())
                {
                    throw new SimpleDB.IOException("Read past end of record at " + Conversions.ToString(this.m_Reader.BaseStream.Position));
                }
                this.m_EntriesRead++;
                byte num = this.m_Reader.ReadByte();
                entry.Type = (SimpleDB.EntryType)num;
                byte num3 = num;
                if (num3 == 0x45)
                {
                    entry.Value = "";
                }
                else if (num3 == 0x42)
                {
                    byte num2 = this.m_Reader.ReadByte();
                    entry.Value = num2 == 1;
                }
                else if (num3 == 0x49)
                {
                    entry.Value = this.RawReadUInt16();
                }
                else if (num3 == 0x53)
                {
                    entry.Value = this.RawReadCString();
                }
                else if (num3 == 0x62)
                {
                    entry.Value = this.m_Reader.ReadByte();
                }
                else
                {
                    entry.Type = SimpleDB.EntryType.Error;
                    entry.Value = "";
                }
                return entry;
            }

            public int RetrieveInt16()
            {
                SimpleDB.Entry entry = this.RetrieveEntry();
                if (entry.Type != SimpleDB.EntryType.UInt16)
                {
                    throw new SimpleDB.IOException(entry.Type, this.m_Reader);
                }
                return Conversions.ToInteger(entry.Value);
            }

            public string RetrieveString()
            {
                SimpleDB.Entry entry = this.RetrieveEntry();
                if (entry.Type != SimpleDB.EntryType.String)
                {
                    throw new SimpleDB.IOException(entry.Type, this.m_Reader);
                }
                return Conversions.ToString(entry.Value);
            }
        }

        public class Writer
        {
            // Fields
            private readonly SimpleDB.EntryList m_currentRecord = new SimpleDB.EntryList();
            private string m_errorDescription;
            private FileStream m_file;
            private BinaryWriter m_writer;

            // Methods
            public void Close()
            {
                this.WriteRecord();
                this.m_file.Close();
            }

            public string ErrorDescription()
            {
                return this.m_errorDescription;
            }

            ~Writer()
            {
                this.Close();
            }

            public void NewRecord()
            {
                this.WriteRecord();
            }

            public void Open(string Path, string Header)
            {
                try
                {
                    this.m_file = new FileStream(Path, FileMode.Create);
                    this.m_writer = new BinaryWriter(this.m_file);
                    this.RawWriteCString(Header);
                }
                catch (Exception exception1)
                {
                    Exception inner = exception1;
                    throw new SimpleDB.IOException("Could not open file", inner);
                }
            }

            private void RawWriteByte(byte Value)
            {
                this.m_writer.Write(Value);
            }

            private void RawWriteCString(string Text)
            {
                int num2 = Text.Length - 1;
                for (int i = 0; i <= num2; i++)
                {
                    this.RawWriteInt16(Text[i]);
                }
                this.RawWriteInt16(0);
            }

            private void RawWriteInt16(int Value)
            {
                byte num = (byte)(Value & 0xff);
                byte num2 = (byte)((Value >> 8) & 0xff);
                this.m_writer.Write(num);
                this.m_writer.Write(num2);
            }

            private void RawWriteInt32(int Value)
            {
                byte num3 = 0;
                byte num4 =0;
                byte num = (byte)(Value & 0xff);
                byte num2 = (byte)((Value >> 8) & 0xff);
                num2 = (byte)((Value >> 0x10) & 0xff);
                num2 = (byte)((Value >> 0x18) & 0xff);
                this.m_writer.Write(num);
                this.m_writer.Write(num2);
                this.m_writer.Write(num3);
                this.m_writer.Write(num4);
            }

            public void StoreBoolean(bool Value)
            {
                this.m_currentRecord.Add(new SimpleDB.Entry(SimpleDB.EntryType.Boolean, Value));
            }

            public void StoreByte(byte Value)
            {
                this.m_currentRecord.Add(new SimpleDB.Entry(SimpleDB.EntryType.Byte, Value));
            }

            public void StoreEmpty()
            {
                this.m_currentRecord.Add(new SimpleDB.Entry(SimpleDB.EntryType.Empty, ""));
            }

            public void StoreInt16(int Value)
            {
                this.m_currentRecord.Add(new SimpleDB.Entry(SimpleDB.EntryType.UInt16, Value));
            }

            public void StoreString(string Value)
            {
                this.m_currentRecord.Add(new SimpleDB.Entry(SimpleDB.EntryType.String, Value));
            }

            private void WriteRecord()
            {
                if (this.m_currentRecord.Count >= 1)
                {
                    this.RawWriteByte(0x4d);
                    this.RawWriteInt16(this.m_currentRecord.Count);
                    int num2 = this.m_currentRecord.Count - 1;
                    for (int i = 0; i <= num2; i++)
                    {
                        SimpleDB.Entry entry = this.m_currentRecord[i];
                        SimpleDB.EntryType type = entry.Type;
                        if (type == SimpleDB.EntryType.Boolean)
                        {
                            this.RawWriteByte(0x42);
                            this.RawWriteByte(Conversions.ToByte(Conversions.ToBoolean(entry.Value) ? 1 : 0));
                        }
                        else if (type == SimpleDB.EntryType.Byte)
                        {
                            this.RawWriteByte(0x62);
                            this.RawWriteByte(Conversions.ToByte(entry.Value));
                        }
                        else if (type == SimpleDB.EntryType.String)
                        {
                            this.RawWriteByte(0x53);
                            SimpleDB.Entry entry2 = entry;
                            string text = Conversions.ToString(entry2.Value);
                            this.RawWriteCString(text);
                            entry2.Value = text;
                        }
                        else if (type == SimpleDB.EntryType.UInt16)
                        {
                            this.RawWriteByte(0x49);
                            this.RawWriteInt16(Conversions.ToInteger(entry.Value));
                        }
                        else
                        {
                            this.RawWriteByte(0x45);
                        }
                    }
                    this.m_currentRecord.Clear();
                }
            }
        }
    }
}