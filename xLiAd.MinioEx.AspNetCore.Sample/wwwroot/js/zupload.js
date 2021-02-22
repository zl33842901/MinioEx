//zupload 上传插件 https://github.com/zl33842901/zupload.js
(function ($) {
    // 插件的定义     
    $.fn.ZUpload = function (options) {
        debug(this);
        var opts = $.extend({}, $.fn.ZUpload.defaults, options);
        if (!window.FormData && opts.alertWhenUnsuport) {
            alert(alertContentWhenUnsuport);
            return;
        }
        var i = 0;
        var files = [];
        return this.each(function () {
            $this = $(this);
            var tid = "FileZUpload" + (i++);
            var thtml = "<input type=\"file\" id=\"" + tid + "\" style=\"visibility:hidden\" " + (opts.multi ? "multiple" : "") + " accept=\"" + opts.accept + "\" />";
            $this.after(thtml)
            //$this.attr("for", tid);
            $this.bind("click", function () {
                $("#" + tid).click();
            });
            $("#" + tid).bind("change", function (e) {
                var selFiles;
                if (e.target.files.length || e.dataTransfer.files.length) {
                    selFiles = e.target.files || e.dataTransfer.files;
                }
                if (selFiles != null && selFiles.length > 0) {
                    if (!checkFileSize(selFiles, opts.fileSize)) {
                        if (opts.funcWhenSize == null) {
                            alert(opts.alertContentWhenSize);
                            return;
                        } else {
                            opts.funcWhenSize();
                            return;
                        }
                    }
                    if (!checkFileExt(selFiles, opts.fileExt)) {
                        if (opts.funcWhenExt == null) {
                            alert(opts.alertContentWhenSize);
                            return;
                        } else {
                            opts.funcWhenExt();
                            return;
                        }
                    }
                    if (!opts.multi)
                        files = [];
                    for (var ii = 0; ii < selFiles.length; ii++)
                        files.push(selFiles[ii]);
                    var cnt = "";
                    for (var index = 0; index < files.length; index++) {
                        cnt += (opts.fileWaitingTmpl.replace("[filename]", selFiles[index].name).replace("[filesize]", Math.round(parseFloat(parseFloat(selFiles[index].size) / 1024), 2) + 'KB'));
                    }
                    if (opts.fileWaitingContainer != null && opts.fileWaitingContainer != "")
                        $(opts.fileWaitingContainer).html(cnt);
                }
            });
            if (opts.buttonUpload != null && opts.buttonUpload != "") {
                $(opts.buttonUpload).bind("click", function () {
                    if (files.length < 1) {
                        if (opts.funcWhenNoFile == null) {
                            if (opts.alertContentWhenNoFile != null && opts.alertContentWhenNoFile != "")
                                alert(opts.alertContentWhenNoFile);
                        } else {
                            opts.funcWhenNoFile();
                        }
                        return;
                    }
                    var oData = new FormData();
                    for (var ii = 0; ii < files.length; ii++)
                        oData.append("Filedata" + ii, files[ii]);
                    for (var ii in opts.params)
                        oData.append(ii, opts.params[ii]);
                    console.log(oData);
                    var oReq = new XMLHttpRequest();
                    oReq.open("POST", opts.url, true);
                    //根据返回状态进行相应处理
                    oReq.onreadystatechange = function (e) {
                        if (oReq.readyState === 4) {
                            if (oReq.status === 200) {
                                opts.onSuccess(oReq.responseText);
                            } else {
                                opts.onFailure(oReq.responseText);
                            }
                        }
                    };
                    oReq.send(oData);
                })
            }

            //// build element specific options     
            //var o = $.meta ? $.extend({}, opts, $this.data()) : opts;
            //// update element styles     
            //$this.css({
            //    backgroundColor: o.background,
            //    color: o.foreground
            //});
            //var markup = $this.html();
            //// call our format function     
            //markup = $.fn.ZUpload.format(markup);
            //$this.html(markup);
        });
    };
    //私有方法
    function debug($obj) {
        if (window.console && window.console.log)
            window.console.log('ZUpload count: ' + $obj.size());
    };
    //检查文件的类型和大小限制
    function checkFileSize(files, fileSize) {
        for (var index = 0; index < files.length; index++) {
            var file = files[index];
            if (file.size > parseFloat(fileSize) * 1024 * 1024) {
                return false;
            }
        }
        return true;
    }
    function checkFileExt(files, fileExt) {
        for (var index = 0; index < files.length; index++) {
            var file = files[index];
            var fileArr = file.name.split('.');
            var fileExtName = fileArr[fileArr.length - 1];
            if (fileExt.indexOf(fileExtName) < 0) {
                return false;
            }
        }
        return true;
    }
    //公有方法
    //$.fn.ZUpload.Upload = function () {
    //    alert(this);
    //    return '';
    //};
    //默认参数
    $.fn.ZUpload.defaults = {
        multi: true, //是否多文件上传
        alertWhenUnsuport: true, //不支持FormData 时是否提示
        alertContentWhenUnsuport: "您的浏览器版本太低,不支持上传功能,请升级浏览器!", //不支持时提示的内容
        fileSize: 1, //文件大小限制，单位（M）
        fileExt: "*.gif;*.png;*.pdf;*.jpg;*.txt;",
        accept: "image/*,text/plain",
        alertContentWhenSize: "文件大小超限了！",
        alertContentWhenExt: "文件格式不符合要求！",
        alertContentWhenNoFile: "您至少需要上传一个文件！",
        funcWhenSize: null,
        funcWhenExt: null,
        funcWhenNoFile: null,
        fileWaitingContainer: "",//待上传文件列表存放在哪个元素
        fileWaitingTmpl: "<span>[filename] [filesize]</span>",//待上传文件列表的内容模板
        buttonUpload: "input",//上传动作的钮
        url: "/",
        params: {},//上传参数
        onSuccess: function (res) { alert(res); },
        onFailure: function (res) { alert(res); }
    };
})(jQuery); 