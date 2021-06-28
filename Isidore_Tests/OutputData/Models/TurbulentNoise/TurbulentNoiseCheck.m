% Data pushed by NoiseFunctionTest.cs
mkBold(1)
% close all; % So I can run it from MatLab

matDir = 'Output';
figDir = matDir;
dircheck({matDir, figDir});

%% Prep
mu = zeros(4, 1);
sig = zeros(4, 1);

%% Standard Normal Turbulent Noise
idx = 1;
mu(idx) = mean(pData(idx, :));
sig(idx) = std(pData(idx, :));
figure
plot(xArr, pData(idx, :))
tstr = sprintf('Normal Noise, mu=%0.3f, sigma=%0.3f', mu(idx), sig(idx));
title(tstr);
disp(tstr)
sStr = 'TurbulentNoiseNorm_mu(idx)0_sig(idx)0';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% Normal Turbulent Noise
idx = idx + 1;
mu(idx) = mean(pData(idx, :));
sig(idx) = std(pData(idx, :));
figure
plot(xArr, pData(idx, :))
tstr = sprintf('Normal Noise, mu=%0.3f, sigma=%0.3f', mu(idx), sig(idx));
title(tstr);
disp(tstr)
sStr = 'TurbulentNoiseNorm_mu(idx)0_sig(idx)5';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% Normal Turbulent Noise
idx = idx + 1;
mu(idx) = mean(pData(idx, :));
sig(idx) = std(pData(idx, :));
figure
plot(xArr, pData(idx, :))
tstr = sprintf('Normal Noise, mu=%0.3f, sigma=%0.3f', mu(idx), sig(idx));
title(tstr);
disp(tstr)
sStr = 'TurbulentNoiseNorm_mu(idx)9_sig(idx)3';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% Log-Normal Turbulent Noise
idx = idx + 1;
parmhat = lognfit(pData(idx, :));
mu(idx) = parmhat(1);
sig(idx) = parmhat(2);
figure
plot(xArr, pData(idx, :))
tstr = sprintf('Log-Normal Noise, mu=%0.3f, sigma=%0.3f', mu(idx), sig(idx));
title(tstr);
disp(tstr)
sStr = 'TurbulentNoiseLogNorm_mu(idx)9_sig(idx)3';
ffile = fullfile(figDir, sStr);
printFig(ffile)

%% Saves
ffile = fullfile(matDir, 'TurbulentNoiseCheck');
save(ffile, 'xArr', 'pData', 'mu', 'sig');

%% Cleanup
old = cd(figDir);
RemoveWhiteBorder;
cd(old)

mkBold(0)