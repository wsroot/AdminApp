using AdminApp.Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using ZhongYan.DingCan.Entities;
using ZhongYan.DingCan.WebApi.Models;

namespace ZhongYan.DingCan.WebApi.Controllers
{
    /// <summary>
    /// 订餐控制器
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    public class DiningRoomController : ControllerBase
    {
        private List<DingCanSettings> DingCanSettings { get; set; }
        public DiningRoomController(IOptions<List<DingCanSettings>> settings)
        {
            DingCanSettings=settings.Value;
        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns></returns>
        [HttpGet]
        [Route("menu/{date}")]
        public ActionResult<Object> Menu(DateTime date)
        {
            var person = new ServiceContext<DingCan_Person>().GetEntity(new { GZZ_BH = GetGzzh() });
            if (person==null)
                return new
                {
                    IsSuccess = false,
                    Msg = "工作证号找不到对应信息"
                };
            var entity = new ServiceContext<DingCan_Menu>().GetEntity(new { riqi = date });
            if (entity==null)
                return new
                {
                    IsSuccess = false,
                    Msg = "当天无菜单信息"
                };
            var now = date.Date;
            var xuze = new ServiceContext<DingCan_XuanZe>().GetEntity(new { RiQi = date,SAP_BH = person.SAP_BH });


            var thisday = DateTime.Now;
            var xuanzelist = new ServiceContext<DingCan_XuanZe>().GetList(new { RiQi = thisday.Date,SAP_BH = person.SAP_BH });

            var zcjs = xuanzelist.Any(x => x.ZhongCan!="N"&&x.RiQi==thisday.Date)&&thisday>thisday.Date.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="ZN").DT);
            var wcjs = xuanzelist.Any(x => x.WanCan!="N"&&x.RiQi==thisday.Date)&&thisday>thisday.Date.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="WN").DT);

            var FrozenScore = 0;
            FrozenScore+=xuanzelist.Where(m=> !GetFree(m.RiQi,person.DanWei)).Count(m => m.ZhongCan!="N")*DingCanSettings.FirstOrDefault(x => x.CX=="ZA").P;
            FrozenScore+=xuanzelist.Where(m => !GetFree(m.RiQi,person.DanWei)).Count(m => m.WanCan!="N")*DingCanSettings.FirstOrDefault(x => x.CX=="WA").P;
            FrozenScore+=xuanzelist.Where(m => !GetFree(m.RiQi,person.DanWei)).Count(m => m.YeCan!="N")*DingCanSettings.FirstOrDefault(x => x.CX=="YA").P;

            if (wcjs&&!GetFree(DateTime.Now.Date,person.DanWei))//当日晚餐已结算
                FrozenScore-=DingCanSettings.FirstOrDefault(x => x.CX=="WA").P;//午晚餐去除一份
            if (zcjs&&!GetFree(DateTime.Now.Date,person.DanWei))//当日午餐已结算
                FrozenScore-=DingCanSettings.FirstOrDefault(x => x.CX=="ZA").P;//午餐去除一份

            var sureFree = GetFree(now, person.DanWei);

            return new
            {
                Z = new List<Object>
                {
                    new
                    {
                        Flag = "A",
                        Name = entity.ZA,
                        Price = sureFree?0:DingCanSettings.FirstOrDefault(x => x.CX=="ZA").P,
                        Enable = now.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="ZA").DT)>=DateTime.Now,
                        Show = DingCanSettings.FirstOrDefault(x => x.CX=="ZA").HM!=0 ? person.MinZu=="回族" : person.MinZu!="回族"
                    },
                    new
                    {
                        Flag = "B",
                        Name = entity.ZB,
                        Price = sureFree?0:DingCanSettings.FirstOrDefault(x => x.CX=="ZB").P,
                        Enable = now.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="ZB").DT)>=DateTime.Now,
                        Show = DingCanSettings.FirstOrDefault(x => x.CX=="ZB").HM!=0 ? person.MinZu=="回族" : person.MinZu!="回族"
                    },
                    new
                    {
                        Flag = "C",
                        Name = entity.ZC,
                        Price = sureFree?0:DingCanSettings.FirstOrDefault(x => x.CX=="ZC").P,
                        Enable = now.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="ZC").DT)>=DateTime.Now,
                        Show = DingCanSettings.FirstOrDefault(x => x.CX=="ZC").HM!=0 ? person.MinZu=="回族" : person.MinZu!="回族"
                    },
                    new
                    {
                        Flag = "D",
                        Name = entity.ZD,
                        Price = sureFree?0:DingCanSettings.FirstOrDefault(x => x.CX=="ZD").P,
                        Enable = now.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="ZD").DT)>=DateTime.Now,
                        Show = DingCanSettings.FirstOrDefault(x => x.CX=="ZD").HM!=0 ? person.MinZu=="回族" : person.MinZu!="回族"
                    },
                    new
                    {
                        Flag = "N",
                        Name = "不订餐",
                        Price = 0,
                        Enable = now.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="ZN").DT)>=DateTime.Now,
                        Show = true
                    }
                },
                W = new List<Object>
                {
                    new
                    {
                        Flag = "A",
                        Name = entity.WA,
                        Price = sureFree?0:DingCanSettings.FirstOrDefault(x => x.CX=="WA").P,
                        Enable = now.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="WA").DT)>=DateTime.Now,
                        Show = DingCanSettings.FirstOrDefault(x => x.CX=="WA").HM!=0 ? person.MinZu=="回族" : person.MinZu!="回族"
                    },
                    new
                    {
                        Flag = "B",
                        Name = entity.WB,
                        Price = sureFree?0:DingCanSettings.FirstOrDefault(x => x.CX=="WB").P,
                        Enable = now.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="WB").DT)>=DateTime.Now,
                        Show = DingCanSettings.FirstOrDefault(x => x.CX=="WB").HM!=0 ? person.MinZu=="回族" : person.MinZu!="回族"
                    },
                    new
                    {
                        Flag = "C",
                        Name = entity.WC,
                        Price = sureFree?0:DingCanSettings.FirstOrDefault(x => x.CX=="WC").P,
                        Enable = now.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="WC").DT)>=DateTime.Now,
                        Show = DingCanSettings.FirstOrDefault(x => x.CX=="WC").HM!=0 ? person.MinZu=="回族" : person.MinZu!="回族"
                    },
                    new
                    {
                        Flag = "D",
                        Name = entity.WD,
                        Price = sureFree?0:DingCanSettings.FirstOrDefault(x => x.CX=="WD").P,
                        Enable = now.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="WD").DT)>=DateTime.Now,
                        Show = DingCanSettings.FirstOrDefault(x => x.CX=="WD").HM!=0 ? person.MinZu=="回族" : person.MinZu!="回族"
                    },
                    new
                    {
                        Flag = "N",
                        Name = "不订餐",
                        Price = 0,
                        Enable = now.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="WN").DT)>=DateTime.Now,
                        Show = true
                    }
                },
                Y = new List<Object>
                {
                    new
                    {
                        Flag = "A",
                        Name = entity.Y,
                        Price = sureFree?0:DingCanSettings.FirstOrDefault(x => x.CX=="YA").P,
                        Enable = now>=DateTime.Now,
                        Show = true
                    },
                    new
                    {
                        Flag = "N",
                        Name = "不订餐",
                        Price = 0,
                        Enable = now>=DateTime.Now,
                        Show = true
                    }
                },
                Order = xuze==null ?
                    new OrderModel { riqi=now.ToShortDateString(),WC="N",YC="N",ZC="N" }
                    : new OrderModel { riqi=xuze.RiQi.ToShortDateString(),ZC=xuze.ZhongCan,WC=xuze.WanCan,YC=xuze.YeCan },
                Person = new { Name = person.XingMing,Score = person.KeYongFenShu,FrozenScore,Station = person.Station_name }
            };
        }

        private bool GetFree(DateTime now,string danWei)
        {
            return new ServiceContext<DingCan_JieRi_MianFei_RiQi>().GetRecord(new { Riqi = now })>=1&&new ServiceContext<DingCan_JieRi_MianFei_DanWei>().GetRecord(new { Danwei = danWei })>=1;
        }

        /// <summary>
        /// 下单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("order")]
        public ActionResult<Object> Order([FromForm]OrderModel order)
        {
            var now = Convert.ToDateTime(order.riqi).Date;
            if (now<DateTime.Now.Date)
                return new
                {
                    Msg = "不允许变更,"+DateTime.Now.ToShortDateString(),
                    IsSuccess = false
                };

            var person = new ServiceContext<DingCan_Person>().GetEntity(new { GZZ_BH = GetGzzh() });
            if (person==null)
                return new
                {
                    Msg = "工作证号找不到对应信息",
                    IsSuccess = false
                };
            if (string.IsNullOrWhiteSpace(person.Station_name))
                return new
                {
                    Msg = "您没有取餐地点，请联系厂内订餐管理员",
                    IsSuccess = false
                };

            var xuanze = new ServiceContext<DingCan_XuanZe>().GetEntity(new { RiQi = order.riqi,SAP_BH = person.SAP_BH });
            if (xuanze!=null)
            {
                var Change = false;
                if (xuanze.ZhongCan!=order.ZC)//变更午餐
                {
                    if (now.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="ZN").DT)<DateTime.Now)
                    {
                        return new
                        {
                            Msg = "该午餐无法变更",
                            IsSuccess = false
                        };
                    }
                    Change=true;
                }
                if (xuanze.WanCan!=order.WC)//变更晚餐
                {
                    if (now.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="WN").DT)<DateTime.Now)
                    {
                        return new
                        {
                            Msg = "该晚餐无法变更",
                            IsSuccess = false
                        };
                    }
                    Change=true;
                }
                if (xuanze.YeCan!=order.YC)//变更夜餐
                {
                    if (now<DateTime.Now)
                    {
                        return new
                        {
                            Msg = "该夜餐无法变更",
                            IsSuccess = false
                        };
                    }
                    Change=true;
                }

                if (Change)
                {

                    var thisday = DateTime.Now;
                    var xuanzelist = new ServiceContext<DingCan_XuanZe>().GetList(new { RiQi = thisday.Date,SAP_BH = person.SAP_BH });

                    xuanzelist.Where(m => m.RiQi==now).ToList().ForEach(m =>
                    {
                        m.DingCanRen=person.XingMing;
                        m.RiQi=Convert.ToDateTime(order.riqi);
                        m.SAP_BH=person.SAP_BH;
                        m.ZhongCan=order.ZC;
                        m.WanCan=order.WC;
                        m.YeCan=order.YC;
                    });

                    var zcjs = xuanzelist.Any(x => x.ZhongCan!="N"&&x.RiQi==thisday.Date)&&thisday>thisday.Date.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="ZN").DT);
                    var wcjs = xuanzelist.Any(x => x.WanCan!="N"&&x.RiQi==thisday.Date)&&thisday>thisday.Date.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="WN").DT);

                    var price = 0;
                    price+=xuanzelist.Where(m => !GetFree(m.RiQi,person.DanWei)).Count(m => m.ZhongCan!="N")*DingCanSettings.FirstOrDefault(x => x.CX=="ZA").P;
                    price+=xuanzelist.Where(m => !GetFree(m.RiQi,person.DanWei)).Count(m => m.WanCan!="N")*DingCanSettings.FirstOrDefault(x => x.CX=="WA").P;
                    price+=xuanzelist.Where(m => !GetFree(m.RiQi,person.DanWei)).Count(m => m.YeCan!="N")*DingCanSettings.FirstOrDefault(x => x.CX=="YA").P;

                    if (wcjs&&!GetFree(DateTime.Now.Date,person.DanWei))
                        price-=DingCanSettings.FirstOrDefault(x => x.CX=="WA").P;
                    if (zcjs&&!GetFree(DateTime.Now.Date,person.DanWei))
                        price-=DingCanSettings.FirstOrDefault(x => x.CX=="ZA").P;

                    if (person.KeYongFenShu<price)
                    {
                        return new
                        {
                            Msg = "卡内剩余积分不足",
                            IsSuccess = false
                        };
                    }
                    var model = new DingCan_XuanZe
                    {
                        DingCanRen=person.XingMing,
                        RiQi=Convert.ToDateTime(order.riqi),
                        SAP_BH=person.SAP_BH,
                        ZhongCan=order.ZC,
                        WanCan=order.WC,
                        YeCan=order.YC
                    };
                    return new
                    {
                        IsSuccess = new ServiceContext<DingCan_XuanZe>().Update(model)
                    };
                }
                return new
                {
                    Msg = "未变更",
                    IsSuccess = false
                };
            }
            else
            {
                if (order.ZC!="N")//变更午餐
                {
                    if (now.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="ZN").DT)<DateTime.Now)
                    {
                        return new
                        {
                            Msg = "已过午餐订餐时间",
                            IsSuccess = false
                        };
                    }
                }
                if (order.WC!="N")//变更晚餐
                {
                    if (now.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="WN").DT)<DateTime.Now)
                    {
                        return new
                        {
                            Msg = "已过夜餐订餐时间",
                            IsSuccess = false
                        };
                    }
                }
                if (order.YC!="N")//变更夜餐
                {
                    if (now<DateTime.Now)
                    {
                        return new
                        {
                            Msg = "已过晚餐订餐时间",
                            IsSuccess = false
                        };
                    }
                }

                var thisday = DateTime.Now;
                var xuanzelist = new ServiceContext<DingCan_XuanZe>().GetList(new { RiQi = thisday.Date,SAP_BH = person.SAP_BH }).ToList();

                xuanzelist.Add(new DingCan_XuanZe
                {
                    DingCanRen=person.XingMing,
                    RiQi=Convert.ToDateTime(order.riqi),
                    SAP_BH=person.SAP_BH,
                    ZhongCan=order.ZC,
                    WanCan=order.WC,
                    YeCan=order.YC
                });

                var zcjs = xuanzelist.Any(x => x.ZhongCan!="N"&&x.RiQi==thisday.Date)&&thisday>thisday.Date.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="ZN").DT);
                var wcjs = xuanzelist.Any(x => x.WanCan!="N"&&x.RiQi==thisday.Date)&&thisday>thisday.Date.AddHours(DingCanSettings.FirstOrDefault(x => x.CX=="WN").DT);

                var price = 0;
                price+=xuanzelist.Where(m => !GetFree(m.RiQi,person.DanWei)).Count(m => m.ZhongCan!="N")*DingCanSettings.FirstOrDefault(x => x.CX=="ZA").P;
                price+=xuanzelist.Where(m => !GetFree(m.RiQi,person.DanWei)).Count(m => m.WanCan!="N")*DingCanSettings.FirstOrDefault(x => x.CX=="WA").P;
                price+=xuanzelist.Where(m => !GetFree(m.RiQi,person.DanWei)).Count(m => m.YeCan!="N")*DingCanSettings.FirstOrDefault(x => x.CX=="YA").P;

                if (wcjs&&!GetFree(DateTime.Now.Date,person.DanWei))
                    price-=DingCanSettings.FirstOrDefault(x => x.CX=="WA").P;
                if (zcjs&&!GetFree(DateTime.Now.Date,person.DanWei))
                    price-=DingCanSettings.FirstOrDefault(x => x.CX=="ZA").P;

                if (person.KeYongFenShu<price)
                {
                    return new
                    {
                        Msg = "卡内剩余积分不足",
                        IsSuccess = false
                    };
                }

                var model = new DingCan_XuanZe
                {
                    DingCanRen=person.XingMing,
                    RiQi=Convert.ToDateTime(order.riqi),
                    SAP_BH=person.SAP_BH,
                    ZhongCan=order.ZC,
                    WanCan=order.WC,
                    YeCan=order.YC
                };
                return new
                {
                    IsSuccess = new ServiceContext<DingCan_XuanZe>().Insert(model)
                };
            }
        }

        private String GetGzzh()
        {
            return Request.Headers ["Authorization"].ToString();
        }
    }
}
