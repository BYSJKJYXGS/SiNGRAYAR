using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XvAitalkModels
{
   public class cwmodel
    {
        public int gm;
        public int id;//定义的语义id
        public int sc;//置信度
        public string w;//识别词
    
    };

    public class wsmodel{
        public int bg;

        public string slot;

        public List<cwmodel>cw;

    }

    public class result{
        public int sn;
        public bool  ls;

        public int bg;

        public int ed;

        public int sc;//置信度

        public List<wsmodel>ws;
    }

    public class awvResult
    {
        public string sst;
        public int id;

        public int score;

        public int bos;

        public int eos;

        public string keyword;
    }
}
