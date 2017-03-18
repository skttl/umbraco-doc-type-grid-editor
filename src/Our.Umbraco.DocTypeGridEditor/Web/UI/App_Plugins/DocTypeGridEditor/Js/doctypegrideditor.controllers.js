angular.module("umbraco").controller("Our.Umbraco.DocTypeGridEditor.GridEditors.DocTypeGridEditor", [

    "$scope",
    "$rootScope",
    "$timeout",
    "$routeParams",
    "editorState",
    'assetsService',
    "Our.Umbraco.DocTypeGridEditor.Services.DocTypeDialogService",
    "Our.Umbraco.DocTypeGridEditor.Resources.DocTypeGridEditorResources",

    function ($scope, $rootScope, $timeout, $routeParams, editorState, assetsService, dtgeDialogService, dtgeResources) {

        $scope.title = "Click to insert item";
        $scope.icon = "icon-item-arrangement";

        $scope.setValue = function (data, callback) {
            $scope.control.value = data;
            if (!("id" in $scope.control.value) || $scope.control.value.id == "") {
                $scope.control.value.id = guid();
            }
            if ("name" in $scope.control.value.value && $scope.control.value.value.name) {
                $scope.title = $scope.control.value.value.name;
            }
            if ("dtgeContentTypeAlias" in $scope.control.value && $scope.control.value.dtgeContentTypeAlias) {
                dtgeResources.getContentTypeIcon($scope.control.value.dtgeContentTypeAlias).then(function (data2) {
                    if (data2.icon) {
                        $scope.icon = data2.icon;
                    }
                });
            }
            if (callback)
                callback();
        };

        $scope.setDocType = function () {
            dtgeDialogService.open({
                editorName: $scope.control.editor.name,
                allowedDocTypes: $scope.control.editor.config.allowedDocTypes || [],
                nameTemplate: $scope.control.editor.config.nameTemplate,
                dialogData: {
                    docTypeAlias: $scope.control.value.dtgeContentTypeAlias,
                    value: $scope.control.value.value
                },
                callback: function (data) {
                    $scope.setValue({
                        dtgeContentTypeAlias: data.docTypeAlias,
                        value: data.value
                    });
                    $scope.setPreview($scope.control.value);
                }
            });
        };

        $scope.setPreview = function (model) {
            if ($scope.control.editor.config && "enablePreview" in $scope.control.editor.config && $scope.control.editor.config.enablePreview) {
                dtgeResources.getEditorMarkupForDocTypePartial(editorState.current.id, model.id,
                    $scope.control.editor.alias, model.dtgeContentTypeAlias, model.value,
                    $scope.control.editor.config.viewPath,
                    $scope.control.editor.config.previewViewPath,
                    !!editorState.current.publishDate)
                    .success(function (htmlResult) {
                        if (htmlResult.trim().length > 0) {
                            $scope.preview = htmlResult;
                        }
                    });
            }
        };

        function init() {
            $timeout(function () {
                if ($scope.control.$initializing) {
                    $scope.setDocType();
                } else if ($scope.control.value) {
                    $scope.setPreview($scope.control.value);
                }
            }, 200);
        }

        if ($scope.control.value) {
            if (!$scope.control.value.dtgeContentTypeAlias && $scope.control.value.docType) {
                $scope.control.value.dtgeContentTypeAlias = $scope.control.value.docType;
            }
            if ($scope.control.value.docType) {
                delete $scope.control.value.docType;
            }
            if (isGuid($scope.control.value.dtgeContentTypeAlias)) {
                dtgeResources.getContentTypeAliasByGuid($scope.control.value.dtgeContentTypeAlias).then(function (data1) {
                    $scope.control.value.dtgeContentTypeAlias = data1.alias;
                    $scope.setValue($scope.control.value, init);
                });
            } else {
                $scope.setValue($scope.control.value, init);
            }
        } else {
            $scope.setValue({
                id: guid(),
                dtgeContentTypeAlias: "",
                value: {}
            }, init);
        }

        // Load preview css / js files
        if ($scope.control.editor.config && "enablePreview" in $scope.control.editor.config && $scope.control.editor.config.enablePreview) {
            if ("previewCssFilePath" in $scope.control.editor.config && $scope.control.editor.config.previewCssFilePath) {
                assetsService.loadCss($scope.control.editor.config.previewCssFilePath, $scope);
            };

            if ("previewJsFilePath" in $scope.control.editor.config && $scope.control.editor.config.previewJsFilePath) {
                assetsService.loadJs($scope.control.editor.config.previewJsFilePath, $scope);
            }
        }

        function guid() {
            function s4() {
                return Math.floor((1 + Math.random()) * 0x10000)
                  .toString(16)
                  .substring(1);
            }
            return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
              s4() + '-' + s4() + s4() + s4();
        }

        function isGuid(input) {
            return new RegExp("^[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12}$", "i").test(input.toString());
        }

    }
]);

angular.module("umbraco").controller("Our.Umbraco.DocTypeGridEditor.Dialogs.DocTypeGridEditorDialog",
    [
        "$scope",
        "$interpolate",
        "formHelper",
        "contentResource",
        "Our.Umbraco.DocTypeGridEditor.Resources.DocTypeGridEditorResources",

        function ($scope, $interpolate, formHelper, contentResource, dtgeResources) {

            $scope.dialogOptions = $scope.$parent.dialogOptions;

            $scope.docTypes = [];
            $scope.dialogMode = "selectDocType";
            $scope.selectedDocType = null;
            $scope.node = null;

            var nameExp = !!$scope.dialogOptions.nameTemplate
                ? $interpolate($scope.dialogOptions.nameTemplate)
                : undefined;

            $scope.selectDocType = function () {
                $scope.dialogMode = "edit";
                $scope.dialogData.docTypeAlias = $scope.selectedDocType.alias;
                loadNode();
            };

            $scope.save = function () {

                // Cause form submitting
                if (formHelper.submitForm({ scope: $scope, formCtrl: $scope.dtgeForm })) {

                    // Copy property values to scope model value
                    if ($scope.node) {
                        var value = {
                            name: $scope.dialogOptions.editorName
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

                        if (nameExp) {
                            var newName = nameExp(value); // Run it against the stored dictionary value, NOT the node object
                            if (newName && (newName = $.trim(newName))) {
                                value.name = newName;
                            }
                        }

                        $scope.dialogData.value = value;
                    } else {
                        $scope.dialogData.value = null;
                    }

                    $scope.submit($scope.dialogData);
                }
            };

            function loadNode() {
                contentResource.getScaffold(-20, $scope.dialogData.docTypeAlias).then(function (data) {
                    // Remove the last tab
                    data.tabs.pop();

                    // Merge current value
                    if ($scope.dialogData.value) {
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
                    $scope.nodeContext = $scope.node = data;
                });
            }

            if ($scope.dialogData.docTypeAlias) {
                $scope.dialogMode = "edit";
                loadNode();
            } else {
                $scope.dialogMode = "selectDocType";
                // No data type, so load a list to choose from
                dtgeResources.getContentTypes($scope.dialogOptions.allowedDocTypes).then(function (docTypes) {
                    $scope.docTypes = docTypes;
                    if ($scope.docTypes.length == 1) {
                        $scope.dialogData.docTypeAlias = $scope.docTypes[0].alias;
                        $scope.dialogMode = "edit";
                        loadNode();
                    }
                });
            }

        }

    ]);