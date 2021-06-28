% Data pushed by TextureMapTest.cs
% Use this to test orientations between C# & MatLab

% close all; % So I can run it from MatLab

matDir = 'Mats';
figDir = 'Figs';
if(~exist(matDir,'dir')); mkdir(matDir); end
if(~exist(figDir,'dir')); mkdir(figDir); end

%% Displays data used to make a texture map
figure 
imagesc(grayImg); axis image;
title('Grayscale C# Image')
printFig([figDir,'\GrayScaleImage'])

figure 
imagesc(grayRes); axis image;
title('Grayscale Resolution Image')
printFig([figDir,'\GrayResImage'])

%% Uses MatLab's image loader
imgMat = imread('..\..\..\..\Isidore_Models\Resources\R.png');
grayMat = sum(double(imgMat),3);

figure 
imagesc(imgMat); axis image;
title('Grayscale MatLab Image')
printFig([figDir,'\GrayMatImage'])

figure 
imagesc(grayRes); axis image;
title('Grayscale MatLab Image')
printFig([figDir,'\GrayMatImage'])

%% Saves
save([matDir,'\TextureMapCheck'])