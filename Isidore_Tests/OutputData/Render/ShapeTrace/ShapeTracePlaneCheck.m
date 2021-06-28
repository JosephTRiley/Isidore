% Data pushed by SphereShapeTrace.cs
% uArr = U coordinates for all tracese
% vArr = V coordinates for all tracese
% pos0 = Grid pixel axis 0 locations [m]
% pos1 = Grid pixel axis 1 locations [m]

% close all; % So I can run it from MatLab

%% Book-keeping
bfOffTag = bfOffIdx + 1; % Handles 0/1 index arrays


%% Displays alpha map
figure 
imagesc(reflectMap); 
axis image;
colorbar
title('Reflection Map Texture')

%% Displays Intersect flag
figure
for idx = 1:12
    subplot(3,4,idx)
    imagesc(pos0, pos1, intArr(:,:,idx));
    axis xy;
    axis image;
    if idx == 2
        title('Intersect');
    end
    if idx == bfOffTag
        title('BackFace Off')
    end 
    colorbar
end

%% Displays cosine incidence angle
figure
for idx = 1:12
    subplot(3,4,idx)
    imagesc(pos0, pos1, cosIncArr(:,:,idx));
    axis xy;
    axis image;
    if idx == 2
        title('CosIncAng');
    end
    if idx == bfOffTag
        title('BackFace Off')
    end
    colorbar
end

%% Displays incidence angle
figure
for idx = 1:12
    subplot(3,4,idx)
    imagesc(pos0, pos1, incAngArr(:,:,idx));
    axis xy;
    axis image;
    if idx == 2
        title('IncAng');
    end
    if idx == bfOffTag
        title('BackFace Off')
    end
    colorbar
end

%% Displays depth
figure
for idx = 1:12
    subplot(3,4,idx)
    imagesc(pos0, pos1, depthArr(:,:,idx));
    axis xy;
    axis image;
    if idx == 2
        title('Depth');
    end
    if idx == bfOffTag
        title('BackFace Off')
    end 
    colorbar
end

%% Displays U coordinates
figure
for idx = 1:12
    subplot(3,4,idx)
    imagesc(pos0, pos1, uArr(:,:,idx));
    axis xy;
    axis image;
    if idx == 2
        title('U');
    end
    if idx == bfOffTag
        title('BackFace Off')
    end
    colorbar
end

%% Displays V coordinates
figure
for idx = 1:12
    subplot(3,4,idx)
    imagesc(pos0, pos1, vArr(:,:,idx));
    axis xy;
    axis image;
    if idx == 2
        title('V');
    end
    if idx == bfOffTag
        title('BackFace Off')
    end
    colorbar
end

%% Displays Reflectance
figure
for idx = 1:12
    subplot(3,4,idx)
    imagesc(pos0, pos1, rArr(:,:,idx));
    axis xy;
    axis image;
    if idx == 2
        title('Reflect');
    end
    if idx == bfOffTag
        title('BackFace Off')
    end
    colorbar
end

%% Saves
save('ShapeTracePlane')