using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{

    /// <summary>
    /// 你永远的大爷
    /// </summary>
    public abstract class TextEncrypt
    {
        /// <summary>
        /// 加密字符数组
        /// </summary>
        /// <param name="sourse">待加密字符数组</param>
        /// <returns>加密过的字符数组</returns>
        public abstract char[] Encode(char[] sourse);
        /// <summary>
        /// 解密字符数组
        /// </summary>
        /// <param name="target">待解密字符数组</param>
        /// <returns>解密后的字符数组</returns>
        public abstract char[] Decode(char[] target);
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="sourse">待加密字符串</param>
        /// <returns>加密过的字符串</returns>
        public string Encode(string sourse)
        {
            return new string(this.Encode(sourse.ToCharArray()));
        }
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="target">待解密字符串</param>
        /// <returns>解密后的字符串</returns>
        public string Decode(string target)
        {
            return new string(this.Decode(target.ToCharArray()));
        }
    }

    /// <summary>
    /// 各个字符高低八位互换类
    /// </summary>
    public class ReverseCharEncrypt : TextEncrypt
    {
        /// <summary>
        /// 字符高低八位互换加密字符数组
        /// </summary>
        /// <param name="sourse">待加密字符数组</param>
        /// <returns>加密过的字符数组</returns>
        public override char[] Encode(char[] sourse)
        {
            char[] chs1 = new char[sourse.Length];
            for (int i = 0; i < sourse.Length; i++)
                chs1[i] = ReverseCharEncrypt.ReverseChar(sourse[i]);
            return chs1;
        }
        /// <summary>
        /// 解密字符高低八位互换字符数组
        /// </summary>
        /// <param name="target">待解密字符数组</param>
        /// <returns>解密后的字符数组</returns>
        public override char[] Decode(char[] target)
        {
            return this.Encode(target);
        }

        /*
         * 
         */
        public static char ReverseChar(char c)
        {
            uint i = (uint)c;
            uint high = i >> 8;         //8~15位
            uint low = i % 256;         //0~7位
            return (char)((low << 8) + high);
        }
    }

    /// <summary>
    /// 各个字符高低八位互换类，且反转整个字符串
    /// </summary>
    public class ReverseStringEncrypt : TextEncrypt
    {
        /// <summary>
        /// 字符高低八位互换且字符串反转加密字符数组
        /// </summary>
        /// <param name="sourse">待加密字符数组</param>
        /// <returns>加密过的字符数组</returns>
        public override char[] Encode(char[] sourse)
        {
            char[] chs1 = new char[sourse.Length];
            for (int i = 0; i < sourse.Length; i++)
                chs1[sourse.Length - i - 1] = ReverseCharEncrypt.ReverseChar(sourse[i]);
            return chs1;
        }
        /// <summary>
        /// 解密字符高低八位互换且字符串反转字符数组
        /// </summary>
        /// <param name="target">待解密字符数组</param>
        /// <returns>解密后的字符数组</returns>
        public override char[] Decode(char[] target)
        {
            return this.Encode(target);
        }
    }

    /// <summary>
    /// 单一字符密钥抽象类
    /// </summary>
    public abstract class SimpleKeyEncrypt : TextEncrypt
    {
        protected ushort _key;

        public SimpleKeyEncrypt(ushort key)
        {
            _key = key;
        }
        /// <summary>
        /// 单一字符密钥加密字符数组
        /// </summary>
        /// <param name="sourse">待加密字符数组</param>
        /// <returns>加密过的字符数组</returns>
        public abstract override char[] Encode(char[] sourse);
        /// <summary>
        /// 解密单一字符密钥字符数组
        /// </summary>
        /// <param name="target">待解密字符数组</param>
        /// <returns>解密后的字符数组</returns>
        public abstract override char[] Decode(char[] target);
    }

    /// <summary>
    /// 单一换字加密类，简单变换ASCII码
    /// </summary>
    public class SimpleChangeEncrpt : SimpleKeyEncrypt
    {
        public SimpleChangeEncrpt(ushort key)
            : base(key)
        { }

        /*
         * 
         */
        /// <summary>
        /// 单一换字加密字符数组
        /// </summary>
        /// <param name="sourse">待加密字符数组</param>
        /// <returns>加密过的字符数组</returns>
        public override char[] Encode(char[] sourse)
        {
            char[] chs1 = new char[sourse.Length];
            for (int i = 0; i < sourse.Length; i++)
                chs1[i] = (char)((sourse[i] + _key) % 65536);
            return chs1;
        }
        /// <summary>
        /// 解密单一换字字符数组
        /// </summary>
        /// <param name="target">待解密字符数组</param>
        /// <returns>解密后的字符数组</returns>
        public override char[] Decode(char[] target)
        {
            char[] chs1 = new char[target.Length];
            for (int i = 0; i < target.Length; i++)
                chs1[i] = (char)((target[i] - _key) % 65536);
            return chs1;
        }
    }

    /// <summary>
    /// 单一异或加密类
    /// </summary>
    public class SimpleXOREncrypt : SimpleKeyEncrypt
    {
        public SimpleXOREncrypt(ushort key)
            : base(key)
        { }
        /// <summary>
        /// 单一异或加密加密字符数组
        /// </summary>
        /// <param name="sourse">待加密字符数组</param>
        /// <returns>加密过的字符数组</returns>
        public override char[] Encode(char[] sourse)
        {
            char[] chs1 = new char[sourse.Length];
            for (int i = 0; i < sourse.Length; i++)
                chs1[i] = (char)(sourse[i] ^ _key);
            return chs1;
        }
        /// <summary>
        /// 解密单一异或加密字符数组
        /// </summary>
        /// <param name="target">待解密字符数组</param>
        /// <returns>解密后的字符数组</returns>
        public override char[] Decode(char[] target)
        {
            return this.Encode(target);
        }
    }

    /// <summary>
    /// 反馈异或加密类
    /// </summary>
    public class FeedBackXOREncrypt : SimpleXOREncrypt
    {
        /*
         * 密文中的一个字符作为下一个明文字符的密钥，防止频率分析破译
         * 但这是否意味着，可以直接破解第一个字符以外的数据？？!!
         */
        public FeedBackXOREncrypt(ushort key)
            : base(key)
        { }
        /// <summary>
        /// 反馈异或加密加密字符数组
        /// </summary>
        /// <param name="sourse">待加密字符数组</param>
        /// <returns>加密过的字符数组</returns>
        public override char[] Encode(char[] sourse)
        {
            char[] chs1 = new char[sourse.Length];
            chs1[0] = (char)(sourse[0] ^ _key);
            for (int i = 1; i < sourse.Length; i++)
                chs1[i] = (char)(chs1[i - 1] ^ sourse[i]);
            return chs1;
        }
        /// <summary>
        /// 解密反馈异或加密字符数组
        /// </summary>
        /// <param name="target">待解密字符数组</param>
        /// <returns>解密后的字符数组</returns>
        public override char[] Decode(char[] target)
        {
            char[] chs1 = new char[target.Length];
            for (int i = target.Length - 1; i > 0; i--)
                chs1[i] = (char)(target[i - 1] ^ target[i]);
            chs1[0] = (char)(target[0] ^ _key);
            return chs1;
        }
    }

    /// <summary>
    /// 多密钥加密器抽象类
    /// </summary>
    public abstract class MultipleKeyEncrypt : TextEncrypt
    {
        protected ushort[] _keys;

        public MultipleKeyEncrypt(params ushort[] keys)
        {
            _keys = new ushort[keys.Length];
            for (int i = 0; i < keys.Length; i++)
                _keys[i] = keys[i];
        }
        /// <summary>
        /// 多密钥加密器抽象类加密抽象方法
        /// </summary>
        /// <param name="sourse">待加密明文字符数组</param>
        /// <returns>加密后的密文字符数组</returns>
        public abstract override char[] Encode(char[] sourse);
        /// <summary>
        /// 多密钥加密器抽象类解密抽象方法
        /// </summary>
        /// <param name="target">带解密密文字符数组</param>
        /// <returns>解密后的明文字符数组</returns>
        public abstract override char[] Decode(char[] target);
    }

    /// <summary>
    /// 多重换字加密算类
    /// </summary>
    public class MultipleChangeEncrypt : MultipleKeyEncrypt
    {
        public MultipleChangeEncrypt(params ushort[] keys)
            : base(keys)
        { }
        //算法原理
        /// <summary>
        /// 多重换字加密字符数组
        /// </summary>
        /// <param name="sourse">待加密字符数组</param>
        /// <returns>加密过的字符数组</returns>
        public override char[] Encode(char[] sourse)
        {
            char[] chs1 = new char[sourse.Length];
            for (int i = 0; i < sourse.Length; i += _keys.Length)
            {
                for (int j = 0; j < _keys.Length; j++)
                {
                    if (i + j < sourse.Length)
                        chs1[i + j] = (char)((sourse[i + j] + _keys[j]) % 65536);
                }
            }
            return chs1;
        }
        //算法原理
        /// <summary>
        /// 解密多重换字字符数组
        /// </summary>
        /// <param name="target">待解密字符数组</param>
        /// <returns>解密后的字符数组</returns>
        public override char[] Decode(char[] target)
        {
            char[] chs1 = new char[target.Length];
            for (int i = 0; i < target.Length; i += _keys.Length)
            {
                for (int j = 0; j < _keys.Length; j++)
                {
                    if (i + j < target.Length)
                        chs1[i + j] = (char)((target[i + j] + 65536 - _keys[j]) % 65536);
                }
            }
            return chs1;
        }
    }

    /// <summary>
    /// 多重异或加密类
    /// </summary>
    public class MultipleXOREncrypt : MultipleKeyEncrypt
    {
        public MultipleXOREncrypt(params ushort[] keys)
            : base(keys)
        { }
        /// <summary>
        /// 多重异或加密字符数组
        /// </summary>
        /// <param name="sourse">待加密字符数组</param>
        /// <returns>加密过的字符数组</returns>
        public override char[] Encode(char[] sourse)
        {
            char[] chs1 = new char[sourse.Length];
            for (int i = 0; i < sourse.Length; i += _keys.Length)
            {
                for (int j = 0; j < _keys.Length; j++)
                {
                    if (i + j < sourse.Length)
                        chs1[i + j] = (char)(sourse[i + j] ^ _keys[j]);
                }
            }
            return chs1;
        }
        /// <summary>
        /// 解密多重异或字符数组
        /// </summary>
        /// <param name="target">待解密字符数组</param>
        /// <returns>解密后的字符数组</returns>
        public override char[] Decode(char[] target)
        {
            return this.Encode(target);
        }
    }

    /// <summary>
    /// 简化版DES加密解密类
    /// </summary>
    public class SDesEncrypt : MultipleKeyEncrypt
    {
        public SDesEncrypt(params ushort[] keys)
            : base(keys)
        {
            if (keys.Length != 4)
                throw new ArgumentException("无效密钥！简化版DES算法应指定4个字密钥");
        }

        //操蛋的看不懂
        /// <summary>
        /// DES加密字符数组
        /// </summary>
        /// <param name="sourse">待加密字符数组</param>
        /// <returns>加密过的字符数组</returns>
        public override char[] Encode(char[] sourse)
        {
            char[] chs1;
            if (sourse.Length % 2 == 0)
                chs1 = new char[sourse.Length];
            else
                chs1 = new char[sourse.Length + 1];

            //操蛋开始
            char tmp, tmp1, tmp2, tmp3, tmp4;
            for (int i = 0; i < sourse.Length; i += 2)
            {
                tmp3 = sourse[i];
                tmp4 = (i + 1 < sourse.Length) ? sourse[i + 1] : ' ';
                //一次变换
                tmp1 = (char)((tmp4 + _keys[0]) % 65536);
                tmp3 ^= tmp1;
                tmp2 = (char)((tmp3 + _keys[1]) % 65536);
                tmp4 ^= tmp2;
                tmp = tmp3;
                tmp3 = ReverseCharEncrypt.ReverseChar(tmp4);
                tmp4 = ReverseCharEncrypt.ReverseChar(tmp);
                //二次变换
                tmp1 = (char)((tmp4 + _keys[2]) % 65536);
                tmp3 ^= tmp1;
                tmp2 = (char)((tmp3 + _keys[3]) % 65536);
                tmp4 ^= tmp2;
                tmp = tmp3;
                tmp3 = ReverseCharEncrypt.ReverseChar(tmp4);
                tmp4 = ReverseCharEncrypt.ReverseChar(tmp);
                //三次变换
                tmp1 = (char)((tmp4 + _keys[3]) % 65536);
                tmp3 ^= tmp1;
                tmp2 = (char)((tmp3 + _keys[2]) % 65536);
                tmp4 ^= tmp2;
                tmp = tmp3;
                tmp3 = ReverseCharEncrypt.ReverseChar(tmp4);
                tmp4 = ReverseCharEncrypt.ReverseChar(tmp);
                //四次变换
                tmp1 = (char)((tmp4 + _keys[1]) % 65536);
                tmp3 ^= tmp1;
                tmp2 = (char)((tmp3 + _keys[0]) % 65536);
                tmp4 ^= tmp2;
                tmp = tmp3;
                tmp3 = tmp4;
                tmp4 = tmp;

                //得到密文
                chs1[i] = (char)tmp3;
                chs1[i + 1] = (char)tmp4;
            }
            return chs1;
        }
        /// <summary>
        /// 解密DES字符数组
        /// </summary>
        /// <param name="target">待解密字符数组</param>
        /// <returns>解密后的字符数组</returns>
        public override char[] Decode(char[] target)
        {
            return this.Encode(target);
        }
    }


    /// <summary>
    /// 文件加密解密类
    /// </summary>
    public class FileEncrypt
    {
        /// <summary>
        /// 单次写入数据长度
        /// </summary>
        protected int Interval = 256;
        /// <summary>
        /// 加密解密类型
        /// </summary>
        protected TextEncrypt _textEncrypt;

        public FileEncrypt(TextEncrypt textEncrypt)
        {
            this._textEncrypt = textEncrypt;
        }
        /// <summary>
        /// 将明文文件加密并生成密文文件
        /// </summary>
        /// <param name="sourceFile">原明文文件路径</param>
        /// <param name="targetFile">加密后的密文文件路径</param>
        /// <param name="encoding">文件数据解码方法</param>
        public virtual void Encrypt(string sourceFile, string targetFile, Encoding encoding)
        {
            FileStream fileStream1 = new FileStream(sourceFile, FileMode.Open);
            FileStream fileStream2 = new FileStream(targetFile, FileMode.Create);
            BinaryReader binaryReader = new BinaryReader(fileStream1, encoding);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream2, encoding);
            char[] buf1;

            while (fileStream1.Position < fileStream1.Length)
            {
                buf1 = binaryReader.ReadChars(Interval);
                binaryWriter.Write(_textEncrypt.Encode(buf1));
            }

            binaryReader.Close();
            binaryWriter.Close();
            fileStream1.Close();
            fileStream2.Close();
        }

        /// <summary>
        /// 将密文文件解密并生成明文文件
        /// </summary>
        /// <param name="sourceFile">原密文文件路径</param>
        /// <param name="targetFile">加密后的明文文件路径</param>
        /// <param name="encoding">文件数据解码方法</param>
        public virtual void Decrypt(string sourceFile, string targetFile, Encoding encoding)
        {
            FileStream fileStream1 = new FileStream(sourceFile, FileMode.Open);
            FileStream fileStream2 = new FileStream(targetFile, FileMode.Create);
            BinaryReader binaryReader = new BinaryReader(fileStream1, encoding);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream2, encoding);
            char[] buf1;

            while (fileStream1.Position < fileStream1.Length)
            {
                buf1 = binaryReader.ReadChars(Interval);
                binaryWriter.Write(_textEncrypt.Decode(buf1));
            }

            binaryReader.Close();
            binaryWriter.Close();
            fileStream1.Close();
            fileStream2.Close();
        }
    }

    /// <summary>
    /// 多线程文件解密加密类
    /// </summary>
    public class TFileEncrypt : FileEncrypt
    {
        /// <summary>
        /// 线程数
        /// </summary>
        protected int _ThreadCount;

        /// <summary>
        /// 构造多线程文件解密加密类
        /// </summary>
        /// <param name="textEncrypt">解密加密方式</param>
        /// <param name="ThreadCount">线程数</param>
        public TFileEncrypt(TextEncrypt textEncrypt, int ThreadCount)
            : base(textEncrypt)
        {
            this._ThreadCount = ThreadCount;
        }

        protected void EncryptFileSegment(string sourseFile, string targetFile,
            long iStart, long iEnd, Encoding encoding)
        {
            FileStream fs1 = new FileStream(sourseFile,
                FileMode.Open, FileAccess.Read, FileShare.Read);
            FileStream fs2 = new FileStream(targetFile,
                FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            BinaryReader br1 = new BinaryReader(fs1, encoding);
            BinaryWriter bw1 = new BinaryWriter(fs2, encoding);
            char[] buf1;

            fs1.Seek(iStart, SeekOrigin.Begin);
            fs2.Seek(iStart, SeekOrigin.Begin);

            while (fs1.Position < iEnd - 1)
            {
                buf1 = br1.ReadChars(Interval);
                if (buf1.Length > 0)
                    bw1.Write(_textEncrypt.Encode(buf1));
            }

            br1.Close();
            bw1.Close();
            fs1.Close();
            fs2.Close();
        }

        protected void DecryptFileSegment(string sourseFile, string targetFile,
            long iStart, long iEnd, Encoding encoding)
        {
            FileStream fs1 = new FileStream(sourseFile,
                FileMode.Open, FileAccess.Read, FileShare.Read);
            FileStream fs2 = new FileStream(targetFile,
                FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            BinaryReader br1 = new BinaryReader(fs1, encoding);
            BinaryWriter bw1 = new BinaryWriter(fs2, encoding);
            char[] buf1;

            fs1.Seek(iStart, SeekOrigin.Begin);
            fs2.Seek(iStart, SeekOrigin.Begin);

            while (fs1.Position < iEnd - 1)
            {
                buf1 = br1.ReadChars(Interval);
                if (buf1.Length > 0)
                    bw1.Write(_textEncrypt.Decode(buf1));
            }

            br1.Close();
            bw1.Close();
            fs1.Close();
            fs2.Close();
        }

        public override void Encrypt(string sourceFile, string targetFile, Encoding encoding)
        {
            FileInfo fi = new FileInfo(sourceFile);
            if (fi.Length <= 4096)
            {
                base.Encrypt(sourceFile, targetFile, encoding);
                return;
            }

            int i;
            int size = (int)(fi.Length / _ThreadCount);
            Thread[] threads = new Thread[_ThreadCount];

            for (i = 0; i < _ThreadCount; i++)
            {
                threads[i] = new Thread(delegate ()
                {
                    this.EncryptFileSegment(sourceFile, targetFile, i * size, (i + 1) * size, encoding);
                });
                threads[i].Start();
            }

            int remain = (int)(fi.Length % _ThreadCount);
            if (remain != 0)
            {
                this.EncryptFileSegment(sourceFile, targetFile, i * size, (i + 1) * size, encoding);
            }

            for (i = 0; i < _ThreadCount; i++)
                threads[i].Join();
        }

        public override void Decrypt(string sourceFile, string targetFile, Encoding encoding)
        {
            FileInfo fi = new FileInfo(sourceFile);
            if (fi.Length <= 4096)
            {
                base.Encrypt(sourceFile, targetFile, encoding);
                return;
            }

            int i;
            int size = (int)(fi.Length / _ThreadCount);
            Thread[] threads = new Thread[_ThreadCount];

            for (i = 0; i < _ThreadCount; i++)
            {
                threads[i] = new Thread(delegate ()
                {
                    this.DecryptFileSegment(sourceFile, targetFile, i * size, (i + 1) * size, encoding);
                });
                threads[i].Start();
            }

            int remain = (int)(fi.Length % _ThreadCount);
            if (remain != 0)
            {
                this.DecryptFileSegment(sourceFile, targetFile, i * size, (i + 1) * size, encoding);
            }

            for (i = 0; i < _ThreadCount; i++)
                threads[i].Join();
        }//end method
    }//end TFileEncrypt
}

