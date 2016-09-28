
// Technique for this controller found at 
//  http://www.c-sharpcorner.com/uploadfile/1d3119/file-upload-and-download-using-html5-file-uploader-control-a851/
// Moved downloading to angular service called fileservices. Was not able to move
// the uploading process to this service.
// Added to the download solution using code found at Stack Overflow. See link below.
MSMApp.controller('mergeController', ['$scope', '$http', '$window', 'FileManager', 'MergeManager',
        function ($scope, $http, $window, FileManager, MergeManager) {
            $scope.mergeStatus = "";

            $scope.QBUploadedFile = FileManager.getQBFileName();
            $scope.APUploadedFile = FileManager.getAPFileName();

            $scope.QBUpload = function () {
                var fd = new FormData()
                for (var i in $scope.files) {
                    fd.append("uploadedFile", $scope.files[i])
                }

                var xhr = new XMLHttpRequest();
                xhr.addEventListener("load", QBUploadComplete, false);
                xhr.open("POST", "http://localhost/MSM/api/FileUploader/UploadFile", true);
                $scope.progressVisible = true;
                xhr.send(fd);
            }

            function QBUploadComplete(evt) {
                $scope.progressVisible = false;
                if (evt.target.status == 201) {
                    $scope.FilePath = evt.target.responseText;

                    $scope.$apply(function (scpe) {
                        $scope.QBUploadStatus = "Upload Complete";
                        console.log($scope.files[0]);
                     //   alert("QB scope.files[0].name = " + $scope.files[0].name);
                        FileManager.setQBUploadFile($scope.files[0]);
                        $scope.QBUploadedFile = FileManager.getQBFileName();
                    })
                }
                else {
                    $scope.$apply(function (scpe) {
                        $scope.UploadStatus = evt.target.responseText;
                    })
                }
            }

            $scope.APUpload = function () {
                var fd = new FormData()
                for (var i in $scope.files) {
                    fd.append("uploadedFile", $scope.files[i])
                }

                var xhr = new XMLHttpRequest();
                xhr.addEventListener("load", APUploadComplete, false);
                xhr.open("POST", "http://localhost/MSM/api/FileUploader/UploadFile", true);
                $scope.progressVisible = true;
                xhr.send(fd);
            }

            function APUploadComplete(evt) {
                $scope.progressVisible = false;
                if (evt.target.status == 201) {
                    $scope.FilePath = evt.target.responseText;

                    $scope.$apply(function (scpe) {
                        $scope.APUploadStatus = "Upload Complete";
                     //   alert("AP scope.files[0].name = " + $scope.files[0].name);
                        FileManager.setAPUploadFile($scope.files[0]);
                        $scope.APUploadedFile = FileManager.getAPFileName();
                    })
                }
                else {
                    $scope.$apply(function (scpe) {
                        $scope.UploadStatus = evt.target.responseText;
                    })
                }
            }

            $scope.fnDownLoad = function () {
                var filePromise = FileManager.getDownloadFile("ImportMe", "csv");

                filePromise.then(function (result) {
                    var textToWrite = result;
                    // alert("download = " + textToWrite);
                    // $window.open(textToWrite);

                    // From: http://stackoverflow.com/questions/34870711/download-a-file-at-different-location-using-html5
                    var textFileAsBlob = new Blob([textToWrite], { type: 'text/plain' });

                    var downloadLink = document.createElement("a");
                    downloadLink.download = "importme.csv";
                    downloadLink.innerHtml = "Download IMPORTME File";

                    if ($window.webkitURL != null) {
                        // alert("Chrome!");
                        downloadLink.href = window.webkitURL.createObjectURL(textFileAsBlob);
                    }
                    else {
                        // alert("Firefox!");
                        // Firefox requires the link to be added to the DOM
                        // before it can be clicked.
                        downloadLink.href = window.URL.createObjectURL(textFileAsBlob);
                        downloadLink.onclick = destroyClickeElement;
                        downloadLink.style.display = "none";
                        document.body.appendChild(downloadLink);
                    }

                    downloadLink.click();

                })
            }

            $scope.GetFileType = function (fileExtension) {
                switch (fileExtension.toLowerCase()) {
                    case "doc":
                    case "docx":
                        $scope.FileType = "application/msword";
                        break;
                    case "xls":
                    case "xlsx":
                        $scope.FileType = "application/vnd.ms-excel";
                        break;
                    case "pps":
                    case "ppt":
                        $scope.FileType = "application/vnd.ms-powerpoint";
                        break;
                    case "txt":
                        $scope.FileType = "text/plain";
                        break;
                    case "rtf":
                        $scope.FileType = "application/rtf";
                        break;
                    case "pdf":
                        $scope.FileType = "application/pdf";
                        break;
                    case "msg":
                    case "eml":
                        $scope.FileType = "application/vnd.ms-outlook";
                        break;
                    case "gif":
                    case "bmp":
                    case "png":
                    case "jpg":
                        $scope.FileType = "image/JPEG";
                        break;
                    case "dwg":
                        $scope.FileType = "application/acad";
                        break;
                    case "zip":
                        $scope.FileType = "application/x-zip-compressed";
                        break;
                    case "rar":
                        $scope.FileType = "application/x-rar-compressed";
                        break;
                }
            }

            $scope.setFiles = function (element) {
                $scope.$apply(function (scpe) {
                    $scope.files = [];
                    for (var i = 0; i < element.files.length; i++) {
                        // alert("files = " + element.files[i]);
                        $scope.files.push(element.files[i]);
                    };
                    $scope.progressVisible = false
                });
            }

            $scope.Merge = function () {
                var qbFileName = FileManager.getQBFileName();
                var qbFileType = FileManager.getQBFileType();

                var apFileName = FileManager.getAPFileName();
                var apFileType = FileManager.getAPFileType();

                if (false && (qbFileName == 'unknown' || apFileName == 'unknown')) {
                    alert("Please upload both a QB file and a AP file");
                }
                else {
                    $scope.mergeStatus = "Merging...";
                    MergeManager.merge(qbFileName, qbFileType, apFileName, apFileType).then(function (ms) {

                   // MergeManager.merge("QB", "XLSX", "AR", "XLSX").then(function (ms) {
                        
                        $scope.mergeStatus = "Completed merge";
                        MergeManager.summarize(ms);
                    });
                }
            }
        }
]);

 

