MergeServices.factory('MergeManager', ['$http', function ($http) {

    var merge = function (qbFileName, qbFileType, apFileName, apFileType) {
        
        if (debugging == true) {
            $http.get("http://localhost/msm/api/merge",
                {
                    params:
                       {
                           "qbFileName": qbFileName,
                           "qbFileType": qbFileType,
                           "apFileName": apFileName,
                           "apFileType": apFileType
                       }
                });
        }

    }
}]);