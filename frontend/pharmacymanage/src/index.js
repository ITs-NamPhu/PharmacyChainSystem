import React from 'react';
import ReactDOM from 'react-dom/client';

import 'bootstrap/dist/css/bootstrap.min.css';
// import 'react-perfect-scrollbar/dist/css/styles.css';
import 'nprogress/nprogress.css';
import './styles/variables.scss';
// import i18n from './utils/i18n';
import Layout from './layout';

import { Spinner } from 'react-bootstrap';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { store, persistor } from './redux/store';
import { Provider } from 'react-redux';
import { PersistGate } from 'redux-persist/integration/react'

const root = ReactDOM.createRoot(document.getElementById('root'));

root.render(

  // provider : share data trong redux xuống các component
  // persistgate: chờ đọc data từ storage
  <Provider store={store}>
    <PersistGate loading={'...loading'} persistor={persistor}>
      <BrowserRouter>
        <Layout />
      </BrowserRouter>
    </PersistGate>
  </Provider>
);
