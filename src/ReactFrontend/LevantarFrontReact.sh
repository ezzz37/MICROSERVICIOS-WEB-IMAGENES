npx create-react-app ReactFrontend

npm install axios react-router-dom

mkdir -p src/assets \
         src/components/CompareModal \
         src/components/ImageProcessor \
         src/components/Dashboard \
         src/components/ImageGallery \
         src/components/Login \
         src/pages \
         src/services \
         src/utils

touch src/components/CompareModal/CompareModal.jsx    src/components/CompareModal/CompareModal.css
touch src/components/ImageProcessor/ImageProcessor.jsx src/components/ImageProcessor/ImageProcessor.css
touch src/components/Dashboard/Dashboard.jsx          src/components/Dashboard/Dashboard.css
touch src/components/ImageGallery/ImageGallery.jsx    src/components/ImageGallery/ImageGallery.css
touch src/components/Login/Login.jsx                  src/components/Login/Login.css

touch src/pages/LoginPage.jsx \
      src/pages/DashboardPage.jsx \
      src/pages/ComparePage.jsx

touch src/services/authService.js src/services/imagenService.js

touch src/utils/apiClient.js

mv public/logo.svg src/assets/logo.svg
