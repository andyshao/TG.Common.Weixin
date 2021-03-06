﻿/*----------------------------------------------------------------
    Copyright (C) 2015 Senparc
    
    文件名：ShakeAroundApi.cs
    文件功能描述：摇一摇周边接口
    
    
    创建标识：Senparc - 20150512
----------------------------------------------------------------*/

/*
    API：http://mp.weixin.qq.com/wiki/15/b9e012f917e3484b7ed02771156411f3.html
 */

using TG.Common.Weixin.Mp;
using TG.Common.Weixin.Mp.Datas;
using System.Collections.Generic;
using System.IO;

namespace TG.Common.Weixin.Mp
{
    /// <summary>
    /// 摇一摇周边接口
    /// </summary>
    public class ShakeAroundApi : MpApi
    {
        public ShakeAroundApi(WxMpApi api) : base(api) { }
        /// <summary>
        /// 申请开通功能
        /// </summary>
        /// <param name="data"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public JsonResult Register(RegisterData data, int timeOut = Config.TIME_OUT)
        {
            var accessToken = _api.GetAccessToken();
            string url = string.Format("https://api.weixin.qq.com/shakearound/account/register?access_token={0}", accessToken);
            return Get<JsonResult>(url, timeOut);
        }

        /// <summary>
        /// 查询审核状态
        /// </summary>
        /// <returns></returns>
        public GetAuditStatusResultJson GetAuditStatus(string accessTokenOrAppId)
        {
            var accessToken = _api.GetAccessToken();
            string url = string.Format("https://api.weixin.qq.com/shakearound/account/auditstatus?access_token={0}", accessToken);
            return Get<GetAuditStatusResultJson>(url);
        }

        /// <summary>
        /// 申请设备ID
        /// </summary>
        /// <param name="quantity">申请的设备ID的数量，单次新增设备超过500个，需走人工审核流程</param>
        /// <param name="applyReason">申请理由，不超过100个字</param>
        /// <param name="comment">备注，不超过15个汉字或30个英文字母</param>
        /// <param name="poiId">设备关联的门店ID，关联门店后，在门店1KM的范围内有优先摇出信息的机会。</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public DeviceApplyResultJson DeviceApply(int quantity, string applyReason, string comment = null, long? poiId = null, int timeOut = Config.TIME_OUT)
        {
            var accessToken = _api.GetAccessToken();
            string url = string.Format("https://api.weixin.qq.com/shakearound/device/applyid?access_token={0}", accessToken);
            var data = new
            {
                quantity = quantity,
                apply_reason = applyReason,
                comment = comment,
                poi_id = poiId
            };
            return Post<DeviceApplyResultJson>(url, data, timeOut);
        }

        /// <summary>
        /// 编辑设备信息
        /// 设备编号，若填了UUID、major、minor，则可不填设备编号，若二者都填，则以设备编号为优先
        /// UUID、major、minor，三个信息需填写完整，若填了设备编号，则可不填此信息。
        /// </summary>
        /// <param name="deviceId">设备编号</param>
        /// <param name="uuId"></param>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="comment"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public JsonResult DeviceUpdate(long deviceId, string uuId, long major, long minor, string comment, int timeOut = Config.TIME_OUT)
        {
            var accessToken = _api.GetAccessToken();
            string url = string.Format("https://api.weixin.qq.com/shakearound/device/update?access_token={0}", accessToken);
            var data = new
            {
                device_identifier = new
                {
                    device_id = deviceId,
                    uuid = uuId,
                    major = major,
                    minor = minor
                },
                comment = comment
            };
            return Post<JsonResult>(url, data, timeOut);
        }

        /// <summary>
        /// 配置设备与门店的关联关系
        /// 设备编号，若填了UUID、major、minor，则可不填设备编号，若二者都填，则以设备编号为优先
        /// UUID、major、minor，三个信息需填写完整，若填了设备编号，则可不填此信息。
        /// </summary>
        /// <param name="deviceId">设备编号</param>
        /// <param name="uuId"></param>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="poiId">Poi_id 的说明改为：设备关联的门店ID，关联门店后，在门店1KM的范围内有优先摇出信息的机会。</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public JsonResult DeviceBindLocatoin(long deviceId, string uuId, long major, long minor, long poiId, int timeOut = Config.TIME_OUT)
        {
            var accessToken = _api.GetAccessToken();
            string url = string.Format("https://api.weixin.qq.com/shakearound/device/bindlocation?access_token={0}", accessToken);
            var data = new
            {
                device_identifier = new
                {
                    device_id = deviceId,
                    uuid = uuId,
                    major = major,
                    minor = minor
                },
                poi_id = poiId
            };
            return Post<JsonResult>(url, data, timeOut);
        }

        #region 查询设备列表

        /// <summary>
        /// 查询设备列表Api url
        /// </summary>
        private string searchDeviceUrl = "https://api.weixin.qq.com/shakearound/device/search?access_token={0}";

        /// <summary>
        /// 根据指定的设备Id查询设备列表
        /// </summary>
        /// <param name="deviceIdentifiers">设备Id列表</param>
        /// 设备编号，若填了UUID、major、minor，则可不填设备编号，若二者都填，则以设备编号为优先
        /// UUID、major、minor，三个信息需填写完整，若填了设备编号，则可不填此信息。
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public DeviceSearchResultJson SearchDeviceById(List<DeviceApply_Data_Device_Identifiers> deviceIdentifiers, int timeOut = Config.TIME_OUT)
        {
            var accessToken = _api.GetAccessToken();
            var url = string.Format(searchDeviceUrl, accessToken);
            var data = new
            {
                device_identifiers = deviceIdentifiers
            };
            return Post<DeviceSearchResultJson>(url, data, timeOut);
        }

        /// <summary>
        /// 根据分页查询或者指定范围查询设备列表
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="count"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public DeviceSearchResultJson SearchDeviceByRange(int begin, int count, int timeOut = Config.TIME_OUT)
        {
            var accessToken = _api.GetAccessToken();
            var url = string.Format(searchDeviceUrl, accessToken);
            var data = new
            {
                begin = begin,
                count = count
            };
            return Post<DeviceSearchResultJson>(url, data, timeOut);
        }

        /// <summary>
        /// 根据批次ID查询设备列表
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="begin"></param>
        /// <param name="count"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public DeviceSearchResultJson SearchDeviceByApplyId(long applyId, int begin, int count, int timeOut = Config.TIME_OUT)
        {
            var accessToken = _api.GetAccessToken();
            var url = string.Format(searchDeviceUrl, accessToken);
            var data = new
            {
                apply_id = applyId,
                begin = begin,
                count = count
            };
            return Post<DeviceSearchResultJson>(url, data, timeOut);
        }

        #endregion

        /// <summary>
        /// 上传图片素材
        /// 上传在摇一摇页面展示的图片素材，素材保存在微信侧服务器上。 格式限定为：jpg,jpeg,png,gif，图片大小建议120px*120 px，限制不超过200 px *200 px，图片需为正方形。
        /// </summary>
        /// <param name="file"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public UploadImageResultJson UploadImage(string file, int timeOut = Config.TIME_OUT)
        {
            var accessToken = _api.GetAccessToken();
            var url = string.Format("https://api.weixin.qq.com/shakearound/material/add?access_token={0}", accessToken);
            var data = new { media = Path.GetFileName(file) };
            return PostFile<UploadImageResultJson>(url, file, data, timeOut);
        }

        /// <summary>
        /// 新增页面
        /// </summary>
        /// <param name="title">在摇一摇页面展示的主标题，不超过6个字</param>
        /// <param name="description">在摇一摇页面展示的副标题，不超过7个字</param>
        /// <param name="pageUrl">点击页面跳转链接</param>
        /// <param name="iconUrl">在摇一摇页面展示的图片。图片需先上传至微信侧服务器，用“素材管理-上传图片素材”接口上传图片，返回的图片URL再配置在此处</param>
        /// <param name="comment">页面的备注信息，不超过15个字</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public AddPageResultJson AddPage(string title, string description, string pageUrl, string iconUrl, string comment = null, int timeOut = Config.TIME_OUT)
        {
            var accessToken = _api.GetAccessToken();
            var url = string.Format("https://api.weixin.qq.com/shakearound/page/add?access_token={0}", accessToken);
            var data = new
            {
                title = title,
                description = description,
                page_url = pageUrl,
                comment = comment,
                icon_url = iconUrl
            };
            return Post<AddPageResultJson>(url, data, timeOut);
        }

        /// <summary>
        /// 编辑页面信息
        /// </summary>
        /// <param name="pageId">摇周边页面唯一ID</param>
        /// <param name="title">在摇一摇页面展示的主标题，不超过6个字</param>
        /// <param name="description">在摇一摇页面展示的副标题，不超过7个字</param>
        /// <param name="pageUrl">点击页面跳转链接</param>
        /// <param name="iconUrl">在摇一摇页面展示的图片。图片需先上传至微信侧服务器，用“素材管理-上传图片素材”接口上传图片，返回的图片URL再配置在此处</param>
        /// <param name="comment">页面的备注信息，不超过15个字</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public UpdatePageResultJson UpdatePage(long pageId, string title, string description, string pageUrl, string iconUrl, string comment = null, int timeOut = Config.TIME_OUT)
        {
            var accessToken = _api.GetAccessToken();
            var url = string.Format("https://api.weixin.qq.com/shakearound/page/update?access_token={0}", accessToken);
            var data = new
            {
                page_id = pageId,
                title = title,
                description = description,
                page_url = pageUrl,
                comment = comment,
                icon_url = iconUrl
            };
            return Post<UpdatePageResultJson>(url, data, timeOut);
        }

        #region 查询页面列表

        private string searchPageUrl = "https://api.weixin.qq.com/shakearound/page/search?access_token={0}";

        /// <summary>
        /// 根据页面Id查询页面列表
        /// </summary>
        /// <param name="pageIds">指定页面的Id数组</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public SearchPages_Data_Page SearchPagesByPageId(long[] pageIds, int timeOut = Config.TIME_OUT)
        {
            var accessToken = _api.GetAccessToken();
            var url = string.Format(searchPageUrl, accessToken);
            var data = new
            {
                page_ids = pageIds
            };
            return Post<SearchPages_Data_Page>(url, data, timeOut);
        }

        /// <summary>
        /// 根据分页或者指定范围查询页面列表
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="count"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public SearchPages_Data_Page SearchPagesByRange(int begin, int count, int timeOut = Config.TIME_OUT)
        {
            var accessToken = _api.GetAccessToken();
            var url = string.Format(searchPageUrl, accessToken);
            var data = new
            {
                begin = begin,
                count = count
            };
            return Post<SearchPages_Data_Page>(url, data, timeOut);
        }

        #endregion

        /// <summary>
        /// 删除页面
        /// </summary>
        /// <param name="pageIds">指定页面的Id数组</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public JsonResult DeletePage(long[] pageIds, int timeOut = Config.TIME_OUT)
        {
            var accessToken = _api.GetAccessToken();
            var url = string.Format("https://api.weixin.qq.com/shakearound/page/delete?access_token={0}", accessToken);
            var data = new
            {
                page_ids = pageIds
            };
            return Post<JsonResult>(url, data, timeOut);
        }

        /// <summary>
        /// 配置设备与页面的关联关系
        /// 配置设备与页面的关联关系。支持建立或解除关联关系，也支持新增页面或覆盖页面等操作。配置完成后，在此设备的信号范围内，即可摇出关联的页面信息。若设备配置多个页面，则随机出现页面信息。一个设备最多可配置30个关联页面。
        /// </summary>
        /// <param name="deviceIdentifier"></param>
        /// 设备编号，若填了UUID、major、minor，则可不填设备编号，若二者都填，则以设备编号为优先
        /// UUID、major、minor，三个信息需填写完整，若填了设备编号，则可不填此信息
        /// <param name="pageIds"></param>
        /// <param name="bindType">关联操作标志位， 0为解除关联关系，1为建立关联关系</param>
        /// <param name="appendType">新增操作标志位， 0为覆盖，1为新增</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public JsonResult BindPage(DeviceApply_Data_Device_Identifiers deviceIdentifier, long[] pageIds, int bindType, int appendType, int timeOut = Config.TIME_OUT)
        {
            var accessToken = _api.GetAccessToken();
            var url = string.Format("https://api.weixin.qq.com/shakearound/device/bindpage?access_token={0}", accessToken);
            var data = new
            {
                device_identifier = deviceIdentifier,
                page_ids = pageIds,
                bind = (int)bindType,
                append = (int)appendType
            };
            return Post<JsonResult>(url, data, timeOut);
        }

        /// <summary>
        /// 获取摇周边的设备及用户信息
        /// </summary>
        /// <param name="ticket">摇周边业务的ticket，可在摇到的URL中得到，ticket生效时间为30分钟，每一次摇都会重新生成新的ticket</param>
        /// <param name="needPoi">是否需要返回门店poi_id，传1则返回，否则不返回</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public GetShakeInfoResultJson GetShakeInfo(string ticket, int needPoi = 1, int timeOut = Config.TIME_OUT)
        {
            var accessToken = _api.GetAccessToken();
            var url = string.Format("https://api.weixin.qq.com/shakearound/user/getshakeinfo?access_token={0}", accessToken);
            var data = new
            {
                ticket = ticket,
                need_poi = needPoi
            };
            return Post<GetShakeInfoResultJson>(url, data, timeOut);
        }

        /// <summary>
        /// 以设备为维度的数据统计接口
        /// </summary>
        /// <param name="deviceIdentifier">指定页面的设备ID</param>
        /// 设备编号，若填了UUID、major、minor，即可不填设备编号，二者选其一
        /// UUID、major、minor，三个信息需填写完成，若填了设备编辑，即可不填此信息，二者选其一
        /// <param name="beginDate">起始日期时间戳，最长时间跨度为30天</param>
        /// <param name="endDate">结束日期时间戳，最长时间跨度为30天</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public StatisticsResultJson StatisticsByDevice(DeviceApply_Data_Device_Identifiers deviceIdentifier, long beginDate, long endDate, int timeOut = Config.TIME_OUT)
        {
            var accessToken = _api.GetAccessToken();
            var url = string.Format("https://api.weixin.qq.com/shakearound/statistics/device?access_token={0}", accessToken);
            var data = new
            {
                device_identifier = deviceIdentifier,
                begin_date = beginDate,
                end_date = endDate
            };
            return Post<StatisticsResultJson>(url, data, timeOut);
        }

        /// <summary>
        /// 以页面为维度的数据统计接口
        /// </summary>
        /// <param name="pageId">指定页面的设备ID</param>
        /// <param name="beginDate">起始日期时间戳，最长时间跨度为30天</param>
        /// <param name="endDate">结束日期时间戳，最长时间跨度为30天</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public StatisticsResultJson StatisticsByPage(long pageId, long beginDate, long endDate, int timeOut = Config.TIME_OUT)
        {
            var accessToken = _api.GetAccessToken();
            var url = string.Format("https://api.weixin.qq.com/shakearound/statistics/page?access_token={0}", accessToken);

            var data = new
            {
                page_id = pageId,
                begin_date = beginDate,
                end_date = endDate
            };
            return Post<StatisticsResultJson>(url, data, timeOut);
        }
    }
}
namespace TG.Common.Weixin
{
    partial class WxMpApi
    {
        private ShakeAroundApi _ShakeAroundApi;
        /// <summary>
        /// 摇一摇周边接口
        /// </summary>
        public ShakeAroundApi ShakeAroundApi
        {
            get
            {
                if (_ShakeAroundApi==null)
                {
                    _ShakeAroundApi = new ShakeAroundApi(this);
                }
                return _ShakeAroundApi;
            }
        }

    }
}