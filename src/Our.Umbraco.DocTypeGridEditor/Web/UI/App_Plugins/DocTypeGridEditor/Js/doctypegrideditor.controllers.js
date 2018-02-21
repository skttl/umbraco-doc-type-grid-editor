angular.module("umbraco").controller("Our.Umbraco.DocTypeGridEditor.GridEditors.DocTypeGridEditor", [

    "$scope",
    "$rootScope",
    "$timeout",
    "$routeParams",
    "editorState",
    'assetsService',
    "Our.Umbraco.DocTypeGridEditor.Resources.DocTypeGridEditorResources",
    "umbRequestHelper",

    function ($scope, $rootScope, $timeout, $routeParams, editorState, assetsService, dtgeResources, umbRequestHelper) {

        $scope.title = "Click to insert item";
        $scope.icon = "icon-item-arrangement";
        $scope.overlay = {};
        $scope.overlay.show = false;
        $scope.overlay.view =
            umbRequestHelper.convertVirtualToAbsolutePath(
                "~/App_Plugins/DocTypeGridEditor/Views/doctypegrideditor.dialog.html");

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

            $scope.overlay = {};
            $scope.overlay.show = true;
            $scope.overlay.title = "Edit item";
            $scope.overlay.submitButtonLabelKey = "buttons_save";
            $scope.overlay.view =
                umbRequestHelper.convertVirtualToAbsolutePath(
                    "~/App_Plugins/DocTypeGridEditor/Views/doctypegrideditor.dialog.html");
            $scope.overlay.editorName = $scope.control.editor.name;
            $scope.overlay.allowedDocTypes = $scope.control.editor.config.allowedDocTypes || [];
            $scope.overlay.nameTemplate = $scope.control.editor.config.nameTemplate;
            $scope.overlay.dialogData = {
                docTypeAlias: $scope.control.value.dtgeContentTypeAlias,
                value: $scope.control.value.value,
                id: $scope.control.value.id
            };
            $scope.overlay.close = function (oldModel) {
                $scope.overlay.show = false;
                $scope.overlay = null;
            }
            $scope.overlay.submit = function (newModel) {

                // Copy property values to scope model value
                if (newModel.node) {
                    var value = {
                        name: newModel.editorName
                    };

                    for (var t = 0; t < newModel.node.tabs.length; t++) {
                        var tab = newModel.node.tabs[t];
                        for (var p = 0; p < tab.properties.length; p++) {
                            var prop = tab.properties[p];
                            if (typeof prop.value !== "function") {
                                value[prop.alias] = prop.value;
                            }
                        }
                    }

                    if (newModel.nameExp) {
                        var newName = newModel.nameExp(value); // Run it against the stored dictionary value, NOT the node object
                        if (newName && (newName = $.trim(newName))) {
                            value.name = newName;
                        }
                    }

                    newModel.dialogData.value = value;
                } else {
                    newModel.dialogData.value = null;

                }

                $scope.setValue({
                    dtgeContentTypeAlias: newModel.dialogData.docTypeAlias,
                    value: newModel.dialogData.value,
                    id: newModel.dialogData.id
                });
                $scope.setPreview($scope.control.value);
                $scope.overlay.show = false;
                $scope.overlay = null;
            };
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
        "Our.Umbraco.DocTypeGridEditor.Services.DocTypeGridEditorUtilityService",

        function ($scope, $interpolate, formHelper, contentResource, dtgeResources, dtgeUtilityService) {

            $scope.docTypes = [];
            $scope.dialogMode = "selectDocType";
            $scope.selectedDocType = null;
            $scope.model.node = null;

            var nameExp = !!$scope.model.nameTemplate
                ? $interpolate($scope.model.nameTemplate)
                : undefined;

            $scope.model.nameExp = nameExp;

            $scope.selectDocType = function () {
                $scope.dialogMode = "edit";
                $scope.model.dialogData.docTypeAlias = $scope.selectedDocType.alias;
                loadNode();
            };

            function loadNode() {
                contentResource.getScaffold(-20, $scope.model.dialogData.docTypeAlias).then(function (data) {

                    if (dtgeUtilityService.compareCurrentUmbracoVersion("7.8", { zeroExtend: true }) < 0) {
                        // Remove the "Generic properties" tab (removed in v7.8)
                        data.tabs.pop();
                    }

                    // Merge current value
                    if ($scope.model.dialogData.value) {
                        for (var t = 0; t < data.tabs.length; t++) {
                            var tab = data.tabs[t];
                            for (var p = 0; p < tab.properties.length; p++) {
                                var prop = tab.properties[p];
                                if ($scope.model.dialogData.value[prop.alias]) {
                                    prop.value = $scope.model.dialogData.value[prop.alias];
                                }
                            }
                        }
                    };

                    // Assign the model to scope
                    $scope.nodeContext = $scope.model.node = data;
                });
            }

            if ($scope.model.dialogData.docTypeAlias) {
                $scope.dialogMode = "edit";
                loadNode();
            } else {
                $scope.dialogMode = "selectDocType";
                // No data type, so load a list to choose from
                dtgeResources.getContentTypes($scope.model.allowedDocTypes).then(function (docTypes) {
                    $scope.docTypes = docTypes;
                    if ($scope.docTypes.length == 1) {
                        $scope.model.dialogData.docTypeAlias = $scope.docTypes[0].alias;
                        $scope.dialogMode = "edit";
                        loadNode();
                    }
                });
            }

        }

    ]);