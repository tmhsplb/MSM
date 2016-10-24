

MSMApp.controller('resolvedController', ['$scope', '$http', '$window', 'FileManager', 'DTOptionsBuilder', 'DTColumnBuilder',
        function ($scope, $http, $window, FileManager, DTOptionsBuilder, DTColumnBuilder) {
            $scope.tab = 'resolved';

            $scope.Download = function () {
                console.log("Donwload");
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

                    if ($window.URL != null) {
                        //  console.log("Download using Chrome");
                        downloadLink.href = window.URL.createObjectURL(textFileAsBlob);
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
        }]);