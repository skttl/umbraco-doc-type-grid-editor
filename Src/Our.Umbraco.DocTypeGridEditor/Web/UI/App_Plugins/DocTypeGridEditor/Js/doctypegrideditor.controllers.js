angular.module("umbraco").controller("Our.Umbraco.DocTypeGridEditor.GridEditors.DocTypeGridEditor", [
    
    "$scope",
    "$rootScope",
    "$timeout",
    "$routeParams",
    "Our.Umbraco.DocTypeGridEditor.Services.DocTypeDialogService",
    "Our.Umbraco.DocTypeGridEditor.Resources.DocTypeGridEditorResources",

    function ($scope, $rootScope, $timeout, $routeParams, dtgeDialogService, dtgeResources) {

        $scope.title = "Click to insert item";
        $scope.icon = "icon-item-arrangement";

        $scope.setValue = function (data) {
            $scope.control.value = data;
            if (!("id" in $scope.control.value) || $scope.control.value.id == "") {
                $scope.control.value.id = guid();
            }
            if ("name" in $scope.control.value.value && $scope.control.value.value.name) {
                $scope.title = $scope.control.value.value.name;
            }
            if ("docType" in $scope.control.value && $scope.control.value.docType) {
                dtgeResources.getContentTypeIcon($scope.control.value.docType).then(function (data2) {
                    if (data2.icon) {
                        $scope.icon = data2.icon;
                    }
                });
            }
        };

        $scope.setDocType = function () {
            dtgeDialogService.open({
                allowedDocTypes: $scope.control.editor.config.allowedDocTypes || [],
                dialogData: {
                    docType: $scope.control.value.docType,
                    value: $scope.control.value.value
                },
                callback: function (data) {
                    $scope.setValue({
                        docType: data.docType,
                        value: data.value
                    });
                    $scope.setPreview($scope.control.value);
                }
            });
        };

        $scope.setPreview = function (model) {
            if ("enablePreview" in $scope.control.editor.config && $scope.control.editor.config.enablePreview) {
                var nodeId = $routeParams.id;
                dtgeResources.getEditorMarkupForDocTypePartial(nodeId, model.id, model.docType, model.value, $scope.control.editor.config.viewPath)
                    .success(function(htmlResult) {
                        if (htmlResult.trim().length > 0) {
                            $scope.preview = htmlResult;
                        }
                    });
            }
        };

        $scope.setValue($scope.control.value || {
            id: guid(),
            docType: "",
            value: {}
        });

        $timeout(function () {
            if ($scope.control.$initializing) {
                $scope.setDocType();
            } else if ($scope.control.value) {
                $scope.setPreview($scope.control.value);
            }
        }, 200);

        function guid() {
            function s4() {
                return Math.floor((1 + Math.random()) * 0x10000)
                  .toString(16)
                  .substring(1);
            }
            return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
              s4() + '-' + s4() + s4() + s4();
        }

    }
]);

angular.module("umbraco").controller("Our.Umbraco.DocTypeGridEditor.Dialogs.DocTypeGridEditorDialog",
    [
        "$scope",
        "editorState",
        "contentResource",
        "Our.Umbraco.DocTypeGridEditor.Resources.DocTypeGridEditorResources",

        function ($scope, editorState, contentResource, dtgeResources) {

            $scope.dialogOptions = $scope.$parent.dialogOptions;

            $scope.docTypes = [];
            $scope.dialogMode = "selectDocType";
            $scope.selectedDocType = null;
            $scope.node = null;
            $scope.nameProperty = {
                hideLabel: false,
                alias: "name",
                label: "Name",
                description: "Give this piece of content a name.",
                value: ""
            };

            $scope.selectDocType = function () {
                $scope.dialogMode = "edit";
                $scope.dialogData.docType = $scope.selectedDocType.guid;
                loadNode();
            };

            $scope.save = function () {

                // Make sure form is valid
                if (!$scope.dtgeForm.$valid)
                    return;

                // Copy property values to scope model value
                if ($scope.node) {
                    var value = {
                        name: $scope.nameProperty.value
                    };
                    for (var t = 0; t < $scope.node.tabs.length; t++) {
                        var tab = $scope.node.tabs[t];
                        for (var p = 0; p < tab.properties.length; p++) {
                            var prop = tab.properties[p];
                            if (typeof prop.value !== "function") {
                                value[prop.alias] = prop.value;
                            }
                        }
                    }
                    $scope.dialogData.value = value;
                } else {
                    $scope.dialogData.value = null;
                }

                $scope.submit($scope.dialogData);
            };

            function loadNode() {
                dtgeResources.getContentAliasByGuid($scope.dialogData.docType).then(function (data1) {
                    contentResource.getScaffold(-20, data1.alias).then(function (data) {
                        // Remove the last tab
                        data.tabs.pop();

                        // Merge current value
                        if ($scope.dialogData.value) {
                            $scope.nameProperty.value = $scope.dialogData.value.name;
                            for (var t = 0; t < data.tabs.length; t++) {
                                var tab = data.tabs[t];
                                for (var p = 0; p < tab.properties.length; p++) {
                                    var prop = tab.properties[p];
                                    if ($scope.dialogData.value[prop.alias]) {
                                        prop.value = $scope.dialogData.value[prop.alias];
                                    }
                                }
                            }
                        };

                        // Assign the model to scope
                        $scope.node = data;

                        editorState.set($scope.node);
                    });
                });
            };

            if ($scope.dialogData.docType) {
                $scope.dialogMode = "edit";
                loadNode();
            } else {
                $scope.dialogMode = "selectDocType";
                // No data type, so load a list to choose from
                dtgeResources.getContentTypes($scope.dialogOptions.allowedDocTypes).then(function (docTypes) {
                    $scope.docTypes = docTypes;
                    if ($scope.docTypes.length == 1) {
                        $scope.dialogData.docType = $scope.docTypes[0].guid;
                        $scope.dialogMode = "edit";
                        loadNode();
                    }
                });
            }

        }

    ]);