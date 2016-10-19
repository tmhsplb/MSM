
MergeServices.factory('MergeManager', ['$http', function ($http) {

    var summary = {};
    summary.matched = 0;
    summary.unmatched = 0;
    summary.merge = "unavailable";

    var merge = function (vcFileName, vcFileType, apFileName, apFileType, qbFileName, qbFileType) {
        if (debugging == true) {
            return $http.get("http://localhost/msm/api/merge",
                 {
                     params:
                        {
                            "vcFileName": vcFileName,
                            "vcFileType": vcFileType,
                            "apFileName": apFileName,
                            "apFileType": apFileType,
                            "qbFileName": qbFileName,
                            "qbFileType": qbFileType
                        }
                 }).then(function (result) {
                     return result.data;
                 });
        }
    };

    var summarize = function(ms) {
        summary.matched = ms.Matched;
        summary.unmatched = ms.Unmatched;
        summary.merge = "available";
    }

    var getMatchedChecks = function() {
        return summary.matched;
    }

    var getUnmatchedChecks = function () {
        return summary.unmatched.length;
    }

    var performedMerge = function()
    {
        return summary.merge == "available";
    }

    return {
        merge: merge,
        summarize: summarize,
        getMatchedChecks: getMatchedChecks,
        getUnmatchedChecks: getUnmatchedChecks,
        performedMerge: performedMerge

    };
}]);