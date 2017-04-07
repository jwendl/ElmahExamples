// GENERAL UTILITY Functions

var urlParams = {};

var _processingDialogData;

var ProcessingDialogData = function(reason, timeoutObj) {
    var self = this;
    self.Reason = reason;
    self.TimeoutObj = timeoutObj;
};

var firstBy = (function () {
    function extend(f) {
        f.thenBy = tb;
        return f;
    }
    function tb(y) {
        var x = this;
        return extend(function (a, b) {
            return x(a, b) || y(a, b);
        });
    }
    return extend;
})();

// Used to allow direct access to the URL query parameters
var match,
    pl = /\+/g,  // Regex for replacing addition symbol with a space
    search = /([^&=]+)=?([^&]*)/g,
    decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
    query = window.location.search.substring(1);

while (match = search.exec(query))
    urlParams[decode(match[1])] = decode(match[2]);


function logError(details) {
    try {
        var redirPath = webPagePath + "LogJavaScriptError";
        var d = new Date();
        var mille = d.getTime();
        var ipAddress = $('#hiddenIpAddress').val();
        $.post(redirPath, { message: mille + ' -- IP: ' + ipAddress + ' -- ' + details });
    } catch (e) {
        console.log("Error Logging Messages to ELMAH - " + e.message);
    }
}

function logMessage(details) {
    try {
        if (enableDetailedLogging) {
            var ipAddress = $('#hiddenIpAddress').val();
            var redirPath = webPagePath + "LogJavaScriptLogMessage";
            var d = new Date();
            var mille = d.getTime();
            $.post(redirPath, { message: mille + ' -- IP: ' + ipAddress + ' -- ' + details });
        }
    } catch (e) {
        console.log("Error Logging Messages to ELMAH - " + e.message);
    }
}

function logJqueryError(baseErrorMessage, xhr, status, errorThrown) {
    try {
        if (enableDetailedLogging) {
            var details;
            if (xhr.responseText != null) {
                var err = eval("(" + xhr.responseText + ")");
                details = baseErrorMessage + ' - Error: ' + err.ExceptionMessage + ' - Exception Type: ' + err.ExceptionType + ' - StackTrace: ' + err.StackTrace;
            } else {
                details = baseErrorMessage + ' - Error Status: ' + xhr.statusText + ' - Jquery Status: ' + status + ' - HTTPError: ' + errorThrown;

            }
            var ipAddress = $('#hiddenIpAddress').val();
            var redirPath = webPagePath + "LogJavaScriptError";
            var d = new Date();
            var mille = d.getTime();
            $.post(redirPath, { message: mille + ' -- IP: ' + ipAddress + ' -- ' + details });
        }
    } catch (e) {
        console.log("Error Logging Messages to ELMAH - " + e.message);
    }
}

window.addEventListener('error', function (evt) {
    var _userId = urlParams['UserId'];
    var message = 'UserId: *' + _userId + '*  - UNHANDLED ERROR - ' + evt.message + ' IN ' + evt.filename + '- Line: ' + evt.lineno;
    console.log(message);
    console.log(evt); // has srcElement / target / etc
    logError(message);
    evt.preventDefault();
    showWindowAlert('An unhandled ERROR occurred. Please contact IT.', 'Se ha producido un error no controlado. Por favor, contacto con IT', redirectUserToLogin, true, null);
    //logMessage(message + " - Redirect to login");
    //window.location.href = webPagePath;
});

function showProcessingDialog(reason) {
    $('#processing').modal({
        opacity: 80,
        modal: true,
        overlayCss: { backgroundColor: "#000" },
        focus: false,
        onShow: function (dialog) {
            if (_processingDialogData != null) {
                clearTimeout(_processingDialogData.TimeoutObj);
                _processingDialogData = null;
            }
            var timeoutObj = setTimeout(
                function () {
                    processDialogTimeout(_processingDialogData);
                }, _processingDialogTimeout);

            _processingDialogData = new ProcessingDialogData(reason, timeoutObj);
        },
        onClose: function (dialog) {
            $.modal.close();

            if (_processingDialogData != null) {
                clearTimeout(_processingDialogData.TimeoutObj);
                _processingDialogData = null;
            }
        }
    });
};

function processDialogTimeout(data) {
    var _userId = urlParams['UserId'];
    logError('UserId: *' + _userId + '* processing dialog timed out. ' + data.Reason);
    $("#processingError").html("Error loading data. Please login again.");
    setTimeout(function () {
        redirectUserToLogin();
    }, 2000);
}

var redirectUserToLogin = function () {
    var redirPath = webPagePath;
    var userId = urlParams['UserId'];
    logMessage('UserId: *' + userId + '*  - ' + "Redirect to login--" + redirPath);
    window.location.href = redirPath;
};

$(document).ready(function () {
    $('#buttonAlertOk').click(function () {
        $.modal.close();
    });
});

function showWindowAlert(labelTextEng, labelTextSp, callbackFunction, closeDialog, paramObj) {
    if (closeDialog) {
        $.modal.close();
    }
    $("#windowAlertLabelEng").text(labelTextEng);
    $("#windowAlertLabelSp").text(labelTextSp);
    $('#windowAlert').modal({
        opacity: 80,
        overlayCss: { backgroundColor: "#000" },
        focus: false,
        onClose: function (dialog) {
            $.modal.close();
            if (callbackFunction != null) {
                callbackFunction(paramObj);
            }
        }
    });
};

getDateticks = function (dateVal) {
    var date = new Date(dateVal);
    
    this.day = date.getDate();
    this.month = date.getMonth() + 1;
    this.year = date.getFullYear();
    this.hour = date.getHours();
    this.minute = date.getMinutes();
    this.second = date.getSeconds();
    this.ms = date.getMilliseconds();
   
    this.monthToDays = function(year, month) {
        var add = 0;
        var result = 0;
        if((year % 4 == 0) && ((year % 100  != 0) || ((year % 100 == 0) && (year % 400 == 0)))) add++;
         
        switch(month) {
            case 0: return 0;
            case 1: result = 31; break;
            case 2: result = 59; break;
            case 3: result = 90; break;
            case 4: result = 120; break;
            case 5: result = 151; break;
            case 6: result = 181; break;
            case 7: result = 212; break;
            case 8: result = 243; break;
            case 9: result = 273; break;
            case 10: result = 304; break;
            case 11: result = 334; break;
            case 12: result = 365; break;
        }
        if(month > 1) result += add;
        return result;      
    }

    this.dateToTicks = function(year, month, day) {
        var a = parseInt((year - 1) * 365);
        var b = parseInt((year - 1) / 4);
        var c = parseInt((year - 1) / 100);
        var d = parseInt((a + - c));
        var e = parseInt((year - 1) / 400);
        var f = parseInt(d + e);
        var monthDays = this.monthToDays(year, month - 1);
        var g = parseInt((f + monthDays) + day);
        var h = parseInt(g - 1);
        return h * 864000000000;
    }

    this.timeToTicks = function(hour, minute, second) {
        return (((hour * 3600) + minute * 60) + second) * 10000000;
    }   
   
    return this.dateToTicks(this.year, this.month, this.day) + this.timeToTicks(this.hour, this.minute, this.second) + (this.ms * 10000);
}

var getWeekDayName = function (dateObjstr) {
    //var partsCurr = dateObjstr.match(/(\d+)/g);
    var iso = dateFormat(dateObjstr, 'isoDateTime', true);
    iso = iso.replace("T00", "T13");
    var newDate = new Date(iso);
    //logMessage("DAY of WEEK - converted: " + newDate);
    var dayName = dateFormat.i18n.dayNames[newDate.getDay()];
    return dayName;

};

var dateFormat = function () {
    var token = /d{1,4}|m{1,4}|yy(?:yy)?|([HhMsTt])\1?|[LloSZ]|"[^"]*"|'[^']*'/g,
		timezone = /\b(?:[PMCEA][SDP]T|(?:Pacific|Mountain|Central|Eastern|Atlantic) (?:Standard|Daylight|Prevailing) Time|(?:GMT|UTC)(?:[-+]\d{4})?)\b/g,
		timezoneClip = /[^-+\dA-Z]/g,
		pad = function (val, len) {
		    val = String(val);
		    len = len || 2;
		    while (val.length < len) val = "0" + val;
		    return val;
		};

    // Regexes and supporting functions are cached through closure
    return function (date, mask, utc) {
        var dF = dateFormat;

        // You can't provide utc if you skip other args (use the "UTC:" mask prefix)
        if (arguments.length == 1 && Object.prototype.toString.call(date) == "[object String]" && !/\d/.test(date)) {
            mask = date;
            date = undefined;
        }
        //logMessage('FORMAT -- Before convert to JS date - ' + date);
        // Passing date through Date applies Date.parse, if necessary
        date = date ? new Date(date) : new Date;
        if (isNaN(date)) throw SyntaxError("invalid date");
        //logMessage('FORMAT -- After convert to JS date - ' + date);
        mask = String(dF.masks[mask] || mask || dF.masks["default"]);

        // Allow setting the utc argument via the mask
        if (mask.slice(0, 4) == "UTC:") {
            mask = mask.slice(4);
            utc = true;
        }

        var _ = utc ? "getUTC" : "get",
			d = date[_ + "Date"](),
			D = date[_ + "Day"](),
			m = date[_ + "Month"](),
			y = date[_ + "FullYear"](),
			H = date[_ + "Hours"](),
			M = date[_ + "Minutes"](),
			s = date[_ + "Seconds"](),
			L = date[_ + "Milliseconds"](),
			o = utc ? 0 : date.getTimezoneOffset(),
			flags = {
			    d: d,
			    dd: pad(d),
			    ddd: dF.i18n.dayNames[D],
			    dddd: dF.i18n.dayNames[D + 7],
			    m: m + 1,
			    mm: pad(m + 1),
			    mmm: dF.i18n.monthNames[m],
			    mmmm: dF.i18n.monthNames[m + 12],
			    yy: String(y).slice(2),
			    yyyy: y,
			    h: H % 12 || 12,
			    hh: pad(H % 12 || 12),
			    H: H,
			    HH: pad(H),
			    M: M,
			    MM: pad(M),
			    s: s,
			    ss: pad(s),
			    l: pad(L, 3),
			    L: pad(L > 99 ? Math.round(L / 10) : L),
			    t: H < 12 ? "a" : "p",
			    tt: H < 12 ? "am" : "pm",
			    T: H < 12 ? "A" : "P",
			    TT: H < 12 ? "AM" : "PM",
			    Z: utc ? "UTC" : (String(date).match(timezone) || [""]).pop().replace(timezoneClip, ""),
			    o: (o > 0 ? "-" : "+") + pad(Math.floor(Math.abs(o) / 60) * 100 + Math.abs(o) % 60, 4),
			    S: ["th", "st", "nd", "rd"][d % 10 > 3 ? 0 : (d % 100 - d % 10 != 10) * d % 10]
			};

        return mask.replace(token, function ($0) {
            return $0 in flags ? flags[$0] : $0.slice(1, $0.length - 1);
        });
    };
}();

// Some common format strings
dateFormat.masks = {
    "default": "ddd mmm dd yyyy HH:MM:ss",
    shortDate: "m/d/yy",
    shortDate2: "mm-dd-yyyy",
    shortDate3: "yyyy-mm-dd",
    shortDateNoDash: "yyyymmdd",
    monthDay: "mm-dd",
    mediumDate: "mmm d, yyyy",
    longDate: "mmmm d, yyyy",
    fullDate: "dddd, mmmm d, yyyy",
    shortTime: "h:MM TT",
    mediumTime: "h:MM:ss TT",
    longTime: "h:MM:ss TT Z",
    isoDate: "yyyy-mm-dd",
    isoTime: "HH:MM:ss",
    isoDateTime: "yyyy-mm-dd'T'HH:MM:ss",
    isoUtcDateTime: "UTC:yyyy-mm-dd'T'HH:MM:ss'Z'",
    dayName: "dddd"
};

// Internationalization strings
dateFormat.i18n = {
    dayNames: [
		"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
    ],
    monthNames: [
		"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
		"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
    ]
};

// For convenience...
Date.prototype.format = function (mask, utc) {
    return dateFormat(this, mask, utc);
};

Date.prototype.setISO8601 = function (dString) {

    var regexp = /(\d\d\d\d)(-)?(\d\d)(-)?(\d\d)(T)?(\d\d)(:)?(\d\d)(:)?(\d\d)(\.\d+)?(Z|([+-])(\d\d)(:)?(\d\d))/;

    if (dString.toString().match(new RegExp(regexp))) {
        var d = dString.match(new RegExp(regexp));
        var offset = 0;

        this.setUTCDate(1);
        this.setUTCFullYear(parseInt(d[1], 10));
        this.setUTCMonth(parseInt(d[3], 10) - 1);
        this.setUTCDate(parseInt(d[5], 10));
        this.setUTCHours(parseInt(d[7], 10));
        this.setUTCMinutes(parseInt(d[9], 10));
        this.setUTCSeconds(parseInt(d[11], 10));
        if (d[12])
            this.setUTCMilliseconds(parseFloat(d[12]) * 1000);
        else
            this.setUTCMilliseconds(0);
        if (d[13] != 'Z') {
            offset = (d[15] * 60) + parseInt(d[17], 10);
            offset *= ((d[14] == '-') ? -1 : 1);
            this.setTime(this.getTime() - offset * 60 * 1000);
        }
    }
    else {
        this.setTime(Date.parse(dString));
    }
    return this;
};

var isDateFormattedMMDDYYY = function (currDate) {
    var isFormatted = false;
    var dateparts = currDate.split("-");
    if (dateparts.length == 3 && dateparts[2].length == 4) {
        isFormatted = true;
    }
    return isFormatted;
};