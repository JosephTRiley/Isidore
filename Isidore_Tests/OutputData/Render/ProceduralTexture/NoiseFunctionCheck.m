% Data pushed by NoiseFunctionTest.cs
mkBold(1)
% close all; % So I can run it from MatLab

matDir = 'MatsNoise';
figDir = 'FigsNoise';
dircheck({matDir, figDir});

%% 1D Perlin Noise
figure
plot(xArr, pData1D)
title('1D Perlin Noise');
sStr = 'PerlinNoise1D';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% 1D Log-Normal Perlin Noise
figure
plot(xArr, lnData1D)
title('1D Log-Normal Perlin Noise');
sStr = 'LogNormalPerlinNoise1D';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% Frequency Noise and Noise array 
figure
plot(time, pnoiseArr, time, fnoiseArr, '--', time, fnoiseArr, ':')
title('Noise Array');
legend('Noise', 'Frequency Noise', 'fBm')
xlim(time([1,end]))
sStr = 'FreqNoise';
ffile = fullfile(figDir, sStr);
printFig(ffile)

% They should have a normal distribution
[pHist, pEdges] = histcounts(pnoiseArr);
[fHist, fEdges] = histcounts(fnoiseArr);
[bHist, fEdges] = histcounts(bnoiseArr);
pCent = pEdges(1:end-1) + diff(pEdges)/2;
pMean = mean(pnoiseArr);
pVar = var(pnoiseArr);
fpHist = max(pHist) * exp(-(pCent - pMean).^2 ./(2 * pVar)); % Normal PDF
% Plot
figure
bar(pCent, [pHist; fHist; bHist])
hold on
plot(pCent, fpHist)
hold off
xlabel('Value')
ylabel('Count')
legend('Noise', 'Frequency Noise', 'fBm', 'PDF Fit')
title('Noise Array Histogram');
sStr = 'FreqNoiseHist';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% Perlin Noise
slen = size(pData);
for idx = 1:slen(3)
    figure
    colormap('gray')
    imagesc(pData(:, :, idx)); 
    axis image;
    axis off;
%     colorbar;
    title(sprintf('Perlin Noise Grid Size %03u', gridSize(idx)));
    sStr = sprintf('PerlinNoiseGridSize%03u', gridSize(idx));
    ffile = fullfile(figDir, sStr);
    printFig(ffile)
end

%% fBm
slen = size(pfData);
for idx = 1:slen(3)
    figure
    colormap('gray')
    imagesc(pfData(:, :, idx)); 
    axis image;
    axis off;
%     colorbar;
    title(sprintf('fBm, H = %01.1f', H(idx)));
    sStr = sprintf('fBmH%01.1f', H(idx));
    sStr = regexprep(sStr, '\.', 'p');
    ffile = fullfile(figDir, sStr);
    printFig(ffile)
end

%% Multiplicative Cascade mBm
slen = size(pmData);
for idx = 1:slen(3)
    figure
    colormap('gray')
    imagesc(pmData(:, :, idx)); 
    axis image;
    axis off;
%     colorbar;
    title(sprintf('Multiplicative Cascade mBm, Offset = %01.1f', ...
        offset(idx)));
    sStr = sprintf('mBmOffset%01.1f', offset(idx));
    sStr = regexprep(sStr, '\.', 'p');
    ffile = fullfile(figDir, sStr);
    printFig(ffile)
end

%% Perlin Turbulence
slen = size(ptData);
for idx = 1:slen(3)
    figure
    colormap('gray')
    imagesc(ptData(:, :, idx)); 
    axis image;
    axis off;
%     colorbar;
    title(sprintf('Perlin Turbulence, \n Maximum Frequency = %03u', maxFreq(idx)));
    sStr = sprintf('PerlinTurbMaxFreq%03u', maxFreq(idx));
    ffile = fullfile(figDir, sStr);
    printFig(ffile)
end

%% Absolute Perlin Turbulence
slen = size(ptData);
for idx = 1:slen(3)
    figure
    colormap('gray')
    imagesc(ptpData(:, :, idx)); 
    axis image;
    axis off;
%     colorbar;
    title(sprintf('Absolute Value Perlin Turbulence \n Maximum Frequency = %03u', maxFreq(idx)));
    sStr = sprintf('AbsPerlinTurbMaxFreq%03u', maxFreq(idx));
    ffile = fullfile(figDir, sStr);
    printFig(ffile)
end

%% Perlin Marble
slen = size(pmmData);
for idx = 1:slen(3)
    figure
    colormap('gray')
    imagesc(pmmData(:, :, idx)); 
    axis image;
    axis off;
%     colorbar;
    title(sprintf('Perlin Marbling, Multiplier = %02u', marbleMult(idx)));
    sStr = sprintf('PerlinMarbleMult%03u', maxFreq(idx));
    ffile = fullfile(figDir, sStr);
    printFig(ffile)
end

%% (Perlin) spectrum noise
figure
colormap('gray')
imagesc(psData(:, :, 1)); 
axis image;
axis off;
%     colorbar;
title('Default (Perlin) Spectrum Noise');
sStr = 'SpectrumNoiseDefault';
ffile = fullfile(figDir, sStr);
printFig(ffile)

figure
colormap('gray')
imagesc(psData(:, :, 2)); 
axis image;
axis off;
%     colorbar;
title('Small Frequency Weighted (Perlin) Spectrum Noise');
sStr = 'SpectrumNoiseWeighted';
ffile = fullfile(figDir, sStr);
printFig(ffile)

figure
colormap('gray')
imagesc(diff(psData, [], 3));
axis image;
axis off;
title('(Perlin) Spectrum Noise Difference');
sStr = 'SpectrumNoiseDiff';
ffile = fullfile(figDir, sStr);
printFig(ffile)

figure
plot(log2(psFreq), log2(psPower))
%     colorbar;
title('Small Frequency Weighted Spectrum');
sStr = 'SpectrumlinWeighted';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% Saves
ffile = fullfile(matDir, 'NoiseFunctionCheck');
save(ffile, 'xArr', 'gridSize', 'pData', 'maxFreq', 'ptData', ...
'ptpData', 'ptpDataComp', 'pfData', 'pfDataComp', 'H', 'pmData', ...
'pmDataComp', 'offset', 'psData', 'psFreq', 'psData', 'pData1D', ...
'lnData1D', 'pnoiseArr', 'fnoiseArr', 'bnoiseArr', 'pmmData', ...
'psData', 'psPower', 'time');

%% Cleanup
old = cd(figDir);
RemoveWhiteBorder;
cd(old)

mkBold(0)