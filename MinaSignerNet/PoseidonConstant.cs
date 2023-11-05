using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace MinaSignerNet
{
    public static class PoseidonConstant
    {
        public static PoseidonConfig PoseidonConfigKimchiFp = new PoseidonConfig()
        {
            Mds = new List<List<BigInteger>>
            {
                new List<BigInteger>
                {
                    BigInteger.Parse("12035446894107573964500871153637039653510326950134440362813193268448863222019"),
                    BigInteger.Parse("25461374787957152039031444204194007219326765802730624564074257060397341542093"),
                    BigInteger.Parse("27667907157110496066452777015908813333407980290333709698851344970789663080149")
                }
            ,
                new List<BigInteger>
                {
                    BigInteger.Parse("4491931056866994439025447213644536587424785196363427220456343191847333476930"),
                    BigInteger.Parse("14743631939509747387607291926699970421064627808101543132147270746750887019919"),
                    BigInteger.Parse("9448400033389617131295304336481030167723486090288313334230651810071857784477")
                },
                new List<BigInteger>
                {
                    BigInteger.Parse("10525578725509990281643336361904863911009900817790387635342941550657754064843"),
                    BigInteger.Parse("27437632000253211280915908546961303399777448677029255413769125486614773776695"),
                    BigInteger.Parse("27566319851776897085443681456689352477426926500749993803132851225169606086988")
                }
            }
        };
    }


}
