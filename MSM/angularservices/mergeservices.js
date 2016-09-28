
MergeServices.factory('MergeManager', ['$http', function ($http) {

    var summary = {};
    summary.matched = 0;
    summary.unmatched = 0;
    summary.merge = "unavailable";

    var merge = function (qbFileName, qbFileType, apFileName, apFileType) {
        if (debugging == true) {
            return $http.get("http://localhost/msm/api/merge",
                 {
                     params:
                        {
                            "qbFileName": qbFileName,
                            "qbFileType": qbFileType,
                            "apFileName": apFileName,
                            "apFileType": apFileType
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

    var getMatched = function() {
        return summary.matched;
    }

    var getUnmatched = function () {
        return summary.unmatched.length;
    }

    var performedMerge = function()
    {
        return summary.merge == "available";
    }

    return {
        merge: merge,
        summarize: summarize,
        getMatched: getMatched,
        getUnmatched: getUnmatched,
        performedMerge: performedMerge

    };
}]);