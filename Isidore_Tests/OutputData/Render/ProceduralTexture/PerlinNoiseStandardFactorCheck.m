% Data pushed by NoiseFunctionTest.cs
mkBold(1)
% close all; % So I can run it from MatLab

matDir = 'MatsPerlinStandard';
figDir = 'FigsPerlinStandard';
dircheck({matDir, figDir});

%% Example 3D Perlin Noise
figure
plot(time, pnoise3DArr)
title('3D Perlin Noise Array');
xlim(time([1,end]))
ylabel('Value')
xlabel('Time')
sStr = 'Perlin3Dvalue';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% Example 4D Perlin Noise
figure
plot(time, pnoise4DArr)
title('4D Perlin Noise Array');
xlim(time([1,end]))
ylabel('Value')
xlabel('Time')
sStr = 'Perlin4Dvalue';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% 3D Frequency Noise and Noise array 
[pHist3, pEdges3] = histcounts(pnoise3DArr, 50);
pCent = pEdges3(1:end-1) + diff(pEdges3)/2;
pMean3 = mean(pnoise3DArr);
pVar3 = var(pnoise3DArr);
fpHist3 = max(pHist3) * exp(-(pCent - pMean3).^2 ./(2 * pVar3));
% Plot
figure
bar(pCent, pHist3)
hold on
plot(pCent, fpHist3)
hold off
xlabel('Value')
ylabel('Count')
legend('Noise', 'PDF Fit')
title('3D Perlin Noise Array Histogram');
sStr = 'Perlin3DNoiseHist';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% 4D Frequency Noise and Noise array 
[pHist4, pEdges4] = histcounts(pnoise4DArr, 50);
pCent = pEdges4(1:end-1) + diff(pEdges4)/2;
pMean4 = mean(pnoise4DArr);
pVar4 = var(pnoise4DArr);
fpHist4 = max(pHist4) * exp(-(pCent - pMean4).^2 ./(2 * pVar4));
% Plot
figure
bar(pCent, pHist4)
hold on
plot(pCent, fpHist4)
hold off
xlabel('Value')
ylabel('Count')
legend('Noise', 'PDF Fit')
title('4D Perlin Noise Array Histogram');
sStr = 'Perlin4DNoiseHist';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% 3D STD
figure
len = 1:length(std3D);
plot(len, std3D)
title('3D Perlin Noise standard deviation');
xlim(len([1,end]))
xlabel('Realization')
ylabel('Standard Deviation')
sStr = 'Perlin3Dstd';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% 4D STD
figure
len = 1:length(std4D);
plot(len, std4D)
title('4D Perlin Noise standard deviation');
xlim(len([1,end]))
xlabel('Realization')
ylabel('Standard Deviation')
sStr = 'Perlin4Dstd';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% Mean STD
mSTD3D = mean(std3D);
mSTD4D = mean(std4D);
stdFac3D = 1 ./ mSTD3D;
stdFac4D = 1 ./ mSTD4D;

fprintf('3D Perlin Standard Factor: f=%0.5f\n', stdFac3D(1))
fprintf('4D Perlin Standard Factor: f=%0.5f\n', stdFac4D(1))

fprintf('Execution Time: w func=%u, w/o func=%u, ratio=%0.5f\n', ...
    eTime, eTime(1) / eTime(2))

%% Saves
ffile = fullfile(matDir, 'PerlinStandardCheck');
save(ffile, 'time', 'pnoise*', 'std*', 'mean*', 'eTime');

%% Cleanup
old = cd(figDir);
RemoveWhiteBorder;
cd(old)

mkBold(0)