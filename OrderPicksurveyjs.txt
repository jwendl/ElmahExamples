// Global Survey Functions
var _userId, _fullName, _questionId;
var _moreSurveyQuestions = false;

function insertSurveyResponse(answer) {
    //var objDef = $.Deferred();
    var apiURL = apiPath + 'Employee/InsertSurveyResponse?userId=' + _userId + "&fullName=" + _fullName + "&answer=" + answer + "&questionId=" + _questionId;

    $.ajax({
        url: apiURL,
        async: false,
        type: 'POST',
        error: function(xhr, status, errorThrown) {
            var baseErrorMessage = 'UserId: *' + _userId + '* Failed to insert user survey response: ' + apiURL;
            logJqueryError(baseErrorMessage, xhr, status, errorThrown);
        },
    });
}

var getUserSurveyQuestions = function(userId) {
    var results = null;
    var retriesMax = 2;
    var retries = 0;

    $.ajax({
        url: apiPath + 'Employee/GetSurveyQuestions?badge=' + userId,
        async: false,
        type: 'GET',
        success: function(jsonData) {
            results = jsonData;
        },
        error: function(xhr, status, errorThrown) {
            var baseErrorMessage = 'UserId: *' + userId + '* - Failed to get user survey questions - URL: ' + apiPath + 'Employee?badge=' + userId;
            logJqueryError(baseErrorMessage, xhr, status, errorThrown);
            if (retries < retriesMax) {
                logMessage('UserId: *' + userId + '* - RETRY get user survey questions - ' + apiPath + 'Employee/GetSurveyQuestions?badge=' + userId);
                retries++;
                $.ajax(this);
            } else {
                logJqueryError(baseErrorMessage, '', '', data.Message);
            }
        },
    });
    return results;
};

function ShowSurvey() {

    $('#windowSurvey').modal({
        persist: true,
        opacity: 80,
        overlayCss: { backgroundColor: "black" },
        focus: false
    });
}

;

var checkUserSurvey = function(userId, fullName) {
    _fullName = fullName;
    _userId = userId;
    var surveyQuestions = getUserSurveyQuestions(userId);
    if (surveyQuestions != null && surveyQuestions.length > 0) {
        if (surveyQuestions.length > 1) {
            _moreSurveyQuestions = true;
        } else {
            _moreSurveyQuestions = false;
        }
        $("#windowSurveyLabelEng").text(surveyQuestions[0].QuestionEnglish);
        $("#windowSurveyLabelSp").text(surveyQuestions[0].QuestionSpanish);
        $("#windowSurveyAddtlLabelEng").text(surveyQuestions[0].AdditionalTextEnglish);
        $("#windowSurveyAddtlLabelSp").text(surveyQuestions[0].AdditionalTextSpanish);
        _questionId = surveyQuestions[0].Id;
        $.modal.close();
        ShowSurvey();
        return true;
    } else {
        return false;
    }
};

var surveyButtonClick = function(event) {
    var answer = false;
    switch (event.srcElement.id) {
    case 'buttonSurveyYes':
        answer = true;
        break;
    case 'buttonSurveyNo':
        answer = false;
        break;
    }
    insertSurveyResponse(answer);
    $.modal.close();
    if (_moreSurveyQuestions) {
        checkUserSurvey(_userId, _fullName);
    }
};
    
$(document).ready(function () {
    $('#buttonSurveyYes').click(function (event) {
        surveyButtonClick(event);
    });

    $('#buttonSurveyNo').click(function (event) {
        surveyButtonClick(event);
    });

});