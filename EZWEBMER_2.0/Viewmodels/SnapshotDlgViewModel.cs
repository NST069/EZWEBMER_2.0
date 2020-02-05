﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace EZWEBMER_2._0.Viewmodels
{
    class SnapshotDlgViewModel : INotifyPropertyChanged
    {
        public Views.IMediaService MediaService { get; set; }

        private Models.VideoInfo _videoinfo;
        public Models.VideoInfo VideoInfo
        {
            get { return _videoinfo; }
            set
            {
                _videoinfo = value;
                OnPropertyChanged(nameof(VideoInfo));
            }
        }

        public SnapshotDlgViewModel(Models.VideoInfo vi)
        {
            VideoInfo = vi;
            List<String> l = new List<String>();
            //https://stackoverflow.com/questions/10631748/mvvm-pattern-violation-mediaelement-play
            foreach (Models.ImageInfo.Formats x in Enum.GetValues(typeof(Models.ImageInfo.Formats)))
                l.Add("." + x);
            AvailableFormats = l;
            SelectedFormat = AvailableFormats[0];
        }

        int _hh;
        public int hh
        {
            get {
                return _hh;
            }
            set
            {
                if (VideoInfo != null)
                {
                    if (value <= VideoInfo.getSeconds() / 3600)
                        _hh = value;
                    else if (value < 0) _hh = 0;
                    else _hh = VideoInfo.getSeconds() / 3600;
                }
                else _hh = 0;
                OnPropertyChanged(nameof(hh));
            }
        }
        int _mm;
        public int mm
        {
            get { return _mm; }
            set
            {
                if (VideoInfo != null)
                {
                    if (value <= VideoInfo.getSeconds() / 60)
                        _mm = value;
                    else if (value < 0) _mm = 0;
                    else _mm = VideoInfo.getSeconds() / 60;
                }
                else _mm = 0;
                OnPropertyChanged(nameof(mm));
            }
        }
        int _ss;
        public int ss
        {
            get { return _ss; }
            set
            {
                if (VideoInfo != null)
                {
                    if (value <= VideoInfo.getSeconds())
                        _ss = value;
                    else if (value < 0) _ss = 0;
                    else _ss = VideoInfo.getSeconds();
                }
                else _ss = 0;
                OnPropertyChanged(nameof(ss));
            }
        }

        List<String> _avformats;
        public List<String> AvailableFormats
        {
            get { return _avformats; }
            set
            {
                _avformats = value;
                OnPropertyChanged(nameof(AvailableFormats));
            }
        }
        String _selectedfmt;
        public String SelectedFormat
        {
            get { return _selectedfmt; }
            set
            {
                _selectedfmt = value;
                OnPropertyChanged(nameof(SelectedFormat));
            }
        }

        public ICommand Snapshot
        {
            get
            {
                return new Models.DelegateCommand((obj) =>
                {
                    Models.FFMpegProcess.GetFrame(VideoInfo.Path, hh, mm, ss, SelectedFormat);
                    
                });
            }
        }

        //UPDATE 
        // hh=MediaService.GetPosition() % 3600;
        // mm=(MediaService.GetPosition() % 3600) % 60;
        // ss=((MediaService.GetPosition() % 3600) % 60) %60;

        public ICommand Loaded
        {
            get
            {
                return new Models.DelegateCommand((obj) =>
                    {
                        this.MediaService = obj as Views.IMediaService;
                        this.MediaService.Load(VideoInfo.Path);
                        this.MediaService.Play();
                        //this.MediaService.Pause();
                    });
            }
        }

        public ICommand Updated {
            get {
                return new Models.DelegateCommand((obj)=> {
                    hh =MediaService.GetPosition() % 3600;
                    mm=(MediaService.GetPosition() % 3600) % 60;
                    ss=((MediaService.GetPosition() % 3600) % 60) %60;
                });
            }
        }
        public ICommand SkipTo {
            get {
                return new Models.DelegateCommand((obj) => {
                    this.MediaService.Stop();
                    this.MediaService.SkipTo(hh * 3600 + mm * 60 + ss);
                    this.MediaService.Play();
                    this.MediaService.Pause();
                });
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]String info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}