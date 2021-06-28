% Data pushed by NoiseFunctionTest.cs
mkBold(1)
% close all; % So I can run it from MatLab

matDir = 'MatsPerlin';
figDir = 'FigsPerlin';
dircheck({matDir, figDir});

%% Original Perlin Noise
figure
colormap('gray')
imagesc(pDataOrig); 
axis image;
axis off;
title('Perlin Noise Original');
sStr = 'PerlinNoiseOrig';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% Original Perlin Noise with Improved (5th-order) interpolation
figure
colormap('gray')
imagesc(pDataImpInt); 
axis image;
axis off;
title('Perlin Noise Original with Improved Interp');
sStr = 'PerlinNoiseImpInt';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% Original Perlin Noise with wide interpolation
figure
colormap('gray')
imagesc(pDataWideInt); 
axis image;
axis off;
title('Perlin Noise Original with Wide Interp');
sStr = 'PerlinNoiseWideInt';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% Original Perlin Noise with radial filter
figure
colormap('gray')
imagesc(pDataRadial); 
axis image;
axis off;
title('Perlin Noise Original with Radial Filter');
sStr = 'PerlinNoiseRadial';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% Improved Perlin Noise
figure
colormap('gray')
imagesc(pDataImpInt); 
axis image;
axis off;
title('Improved Perlin Noise');
sStr = 'PerlinNoiseImprove';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% Perlin Noise Value
figure
colormap('gray')
imagesc(pDataValue); 
axis image;
axis off;
title('Perlin Noise Value');
sStr = 'PerlinNoiseValue';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% Perlin Noise with jitter
figure
colormap('gray')
imagesc(pDataJit); 
axis image;
axis off;
title('Perlin Noise with Jitter');
sStr = 'PerlinNoiseJit';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% Saves
ffile = fullfile(matDir, 'NoiseFunctionCheck');
save(ffile, 'xPos', 'pDataOrig');

%% Cleanup
old = cd(figDir);
RemoveWhiteBorder;
cd(old)

mkBold(0)