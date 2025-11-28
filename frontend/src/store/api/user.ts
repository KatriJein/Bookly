import axios from 'axios';

const apiUrl = 'http://localhost:8082/';

export type TLoginData = {
  login: string;
  password: string;
};

export type TRegisterData = {
  login: string;
  email: string;
  password: string;
};

export type TAuthResponse = {
  id: string;
  login: string;
  email: string;
  avatarUrl?: string;
  accessToken: string;
};

// Логин
export const loginApi = async (
  data: TLoginData
): Promise<TAuthResponse> => {
  try {
    const response = await axios.post<TAuthResponse>(
      `${apiUrl}api/auth/login`,
      data,
      {
        headers: {
          Accept: 'application/json',
          'Content-Type': 'application/json',
        },
      }
    );

    console.log(response.data, 'loginApi');
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      if (typeof error.response?.data === 'string') {
        throw new Error(error.response.data);
      }

      if (error.response?.data?.message) {
        throw new Error(error.response.data.message);
      }

      throw new Error('Login failed');
    }
    throw new Error('Unknown error occurred');
  }
};

// Регистрация
export const registerApi = async (
  data: TRegisterData
): Promise<TAuthResponse> => {
  try {
    const response = await axios.post<TAuthResponse>(
      `${apiUrl}api/auth/register`,
      data,
      {
        headers: {
          Accept: 'application/json',
          'Content-Type': 'application/json',
        },
      }
    );

    console.log(response.data, 'registerApi');
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      if (typeof error.response?.data === 'string') {
        throw new Error(error.response.data);
      }

      if (error.response?.data?.message) {
        throw new Error(error.response.data.message);
      }

      throw new Error('Registration failed');
    }
    throw new Error('Unknown error occurred');
  }
};