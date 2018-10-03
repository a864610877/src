(function ($) {
   
    $.extend($, {
        openLoading: function (msg) {
            var loading = $("#my-modal-loading");
            $(loading).find(".am-modal-hd").html("");
            $(loading).find(".am-modal-hd").html(msg);
            loading.modal("open");
        },
        colseLoading: function () {
            var loading = $("#my-modal-loading");
            $(loading).find(".am-modal-hd").html("");
            loading.modal("close");
        },
        GetstrTrs: function (data, listVal, CheckBoxS) {
            $("#tbodysNum tr").remove();
            if (data.tables.length > 0) {
                var strTr = "";
                for (var i = 0; i < data.tables.length; i++) {
                    strTr += "<tr>";
                    var item = data.tables[i];
                    for (var j = 0; j < listVal.length; j++) {
                        if (CheckBoxS != null && CheckBoxS != "undefined" && item.hasOwnProperty(listVal[0]) && j == 0) {
                            if (listVal[0] == CheckBoxS)
                                strTr += "<td><input type='checkbox' value='" + item[listVal[0]] + "' /></td>";
                        }
                        else {
                            if (item.hasOwnProperty(listVal[j])) {
                                if (!isNaN(item[listVal[j]])) {
                                    strTr += "<td>" + (item[listVal[j]] != null ? item[listVal[j]] : " ") + "</td>";
                                }
                                else if (item[listVal[j]].hasOwnProperty("Action")) {
                                    var AccountsName = item[listVal[j]];
                                    if (AccountsName != null && AccountsName != "undefined") {
                                        strTr += "<td><a href=/" + AccountsName.Controller + "/" + AccountsName.Action + "/" + AccountsName.Key + ">" + AccountsName.Text + "</a></td>"
                                    }
                                }
                                else {
                                    if (item[listVal[j]] != null && item[listVal[j]] != "undefined" && item[listVal[j]].indexOf("/Date") == 0) {
                                        strTr += "<td>" + ChangeDateFormat(item[listVal[j]]) + "</td>";

                                    } else {
                                        strTr += "<td>" + (item[listVal[j]] != null ? item[listVal[j]] : " ") + "</td>";
                                    }
                                }

                            }
                        }
                    }
                    strTr += "</tr>";
                }
                $("#tbodysNum").append(strTr);
            }
            if (data.html) {
                $("#pagehtml").html("");
                $("#pagehtml").append(data.html);
            }
        },

        table_select: function () {
            var strId = "";
            $("#tbodysNum").find("input[type='checkbox']").each(function () {
                var Status = $(this).is(":checked");
                if(Status)
                {
                    var id = $(this).val();
                    strId += id + ",";
                }
            });
            return strId;
        }
    });
    $(document).on('click', "#table_select", function () {
        var selectBox = $("#tbodysNum").find("input[type='checkbox']");
        var CkAllStatus = $(this).is(":checked");
        //var checked = selectBox.attr("checked");
        if (CkAllStatus) {
            selectBox.prop("checked", "checked");
        }
        else {
            selectBox.removeAttr("checked");
        }
    });

    $(document).on('click', "[data-link]", function () {
        $this = $(this);
        $type = $($this).attr("data-type");//类型
        $url = $($this).attr("data-url");//地址
        switch ($type)
        {
            case "jump"://跳转
                window.location.href = $url;
                break;
            case "deletes"://删除
                //window.location.href = $url + "?strIds";
                $.ajax({
                    url: "",
                    data: { strIds: "" },
                    type: "post",
                    dataType: "json",
                    success: function (data) {
                         
                    }
                });
                break;
        }

    });

}(jQuery));